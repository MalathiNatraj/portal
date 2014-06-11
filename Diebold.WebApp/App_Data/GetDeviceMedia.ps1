param([string]$mediaType='{mediaType}', [string]$fileDirectory='{fileDirectory}', [string]$mediaOID='{mediaOID}')
set-item -force -path env:PGPASSWORD -value "reKu2E4r"
if($mediaType -like ''){
	"Invalid mediaType" | out-string
	exit -1
}

if($fileDirectory -like ''){
	"Invalid fileDirectory" | out-string
	exit -1
}

if($mediaOID -like ''){
	"Invalid mediaOID" | Out-String
	exit -1
}
	
$psql_exe = "C:\Program Files (x86)\pgAdmin III\1.16\psql.exe"
$psql_host = "10.79.15.35"
$psql_port = "5432"
$psql_user = "ssvid"
$psql_db = "frontel"

$frontel_exe = "C:\Frontel2\bin\FrontelVideoConv.exe"

"mediaType: $mediaType" | Out-String
if($mediaType -like 'image'){
	"Processing Image Media" | Out-String
	$filePath = "$fileDirectory\\$mediaOID.jpg"
	$psql_cmd = "\lo_export $mediaOID '$filePath'"
	$psql_cmd | Out-String
	& $psql_exe -h $psql_host -p $psql_port -d $psql_db -U $psql_user -c "$psql_cmd" | Out-String
	if(Test-Path ($filePath)){
		"CAPTURE_SUCCESS" | Out-String
		exit 1
	}
	exit -1
}

if($mediaType -like 'video'){

	"Processing Video Media" | Out-String
	$filePath = "$fileDirectory\$mediaOID.mpg"
	$frontel_cmd = """$filePath"" /oid=$mediaOID /v=mpg /DBHost=$psql_host"
	$frontel_exe | Out-String
	$frontel_cmd | Out-String
	& $frontel_exe $frontel_cmd | Out-String
	if(Test-Path ($filePath)){
		"CAPTURE_SUCCESS" | Out-String
		exit 1
	}
	
	exit -1
}

if($mediaType -like 'mas'){
	"Processing Image Media" | Out-String
	$filePath = "$fileDirectory\\$mediaOID.jpg"
	$psql_cmd = "SELECT ev_file FROM events WHERE ev_type='9001' and cnx_id = $mediaOID"
	$psql_cmd | Out-String
	$mas_cmd = "-h $psql_host", "-p $psql_port", "-d $psql_db", "-U $psql_user", "-c ""$psql_cmd"""
	$mas_cmd | Out-String
	
	$psi = New-object System.Diagnostics.ProcessStartInfo
	$psi.CreateNoWindow = $true
	$psi.UseShellExecute = $false
	$psi.RedirectStandardOutput = $true
	$psi.RedirectStandardError = $true
	$psi.FileName = $psql_exe
	$psi.Arguments = $mas_cmd
	$process = New-Object System.Diagnostics.Process
	$process.StartInfo = $psi
	$process.Start() | Out-Null
	$process.WaitForExit()
	$output = $process.StandardOutput.ReadToEnd()
	
	
	$output | Out-String
	"CAPTURE_SUCCESS" | Out-String
		
	exit -1
}
