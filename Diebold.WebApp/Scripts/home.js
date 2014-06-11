// CardHolder Modify
var CHid;
var CHfirstName;
var CHlastName;
var CHNumber;
var CHListPin;
var CHListActivationDate;
var CHListexpirationDate;
var CHListisActive;
var CHListaccessGroupId;

//UserCode Modify
var AccessUserName;
var AccessUserCode;
var AccessUserNumber;
var AccessProfileNumber;
//UserCode Delete
var DeletedProfileNumber
// Weather Alert;
var WeatherAlertMessage;
var SelectedState;
var SelectedCity;
var infowindow;
// dashboard
// Site Note
var GlobalSiteNoteId;
// Company Inventory
var SeletedCompanyInventoryIndex;
var intCompanyInventoryId;
function getWidthofColumn() {
    document.getElementById("emptyPlaceHolderLeft").style.paddingTop = "0px";
    document.getElementById("emptyPlaceHolderRight").style.paddingTop = "0px";
    $('div#Column2').css("min-height", "");
    var leftheight = document.getElementById("Column1").offsetHeight;
    var midheight = document.getElementById("Column2").offsetHeight;
    var rightheight = document.getElementById("Column3").offsetHeight;
    var overallheight = midheight;

    if (leftheight > midheight && leftheight > rightheight) {
        overallheight = leftheight;
    }
    else if (midheight > leftheight && midheight > rightheight) {
        overallheight = midheight;
    }
    else {
        overallheight = rightheight;
    }
    var rightplaceholdersize = overallheight - rightheight;
    var leftplaceholdersize = overallheight - leftheight;

    var rightplaceholderheight = rightplaceholdersize.toString() + "px";
    var leftplaceholderheight = leftplaceholdersize.toString() + "px";
    var midheightvalue = overallheight.toString() + "px";
    if (rightplaceholdersize < 100) {
        rightplaceholderheight = "100px";
        overallheight = overallheight + (75 - rightplaceholdersize);
        midheightvalue = overallheight.toString() + "px";
        leftplaceholderheight = (leftplaceholdersize + (75 - rightplaceholdersize)).toString() + "px";
    }
    if (leftplaceholdersize < 100) {
        leftplaceholderheight = "100px";
        overallheight = overallheight + (75 - leftplaceholdersize);
        midheightvalue = overallheight.toString() + "px";
        rightplaceholderheight = (rightplaceholdersize + (75 - leftplaceholdersize)).toString() + "px";
    }

    document.getElementById("Column2").style.minHeight = midheightvalue;
    document.getElementById("emptyPlaceHolderLeft").style.paddingTop = leftplaceholderheight;
    document.getElementById("emptyPlaceHolderRight").style.paddingTop = rightplaceholderheight;

}

// Site Information
function DeleteSiteNote(SiteId) {
    //    if (document.getElementById('hdnUserRole').value != 'General Administrator' && document.getElementById('hdnUserRole').value != 'Diebold Administrator') {
    //        alert('Only Administrator has rights to delete site notes');
    //        return false;
    //    }
    //    else {
    var wnd = $("#DeleteNote").data("kendoWindow");
    wnd.content("<p style='height:100px'>Are you sure you want to delete this item?</p><p>" +
            "<input type='button' value='Ok' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-right:4px;' onclick='javascript:ConfirmDeleteNoteOk(" + SiteId + ");'>" +
                "<input type='button' value='Cancel' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-left:5px;' onclick='ConfirmDeleteNoteCancel()'></p>");
    wnd.center().open();
    //    }
}

function ConfirmDeleteNoteCancel() {
    $("#DeleteNote").data("kendoWindow").close();
}
function ConfirmDeleteNoteOk(SiteId) {
    $("#DeleteNote").data("kendoWindow").close();
    var parameters = { "SiteNoteId": SiteId, "LocationId": $("#LocationList").data("kendoComboBox").value() };
    $.post(siteRemoveSiteNote, parameters, RemoveSiteNoteCallback, "json");
}

function RemoveSiteNoteCallback(SiteNoteStatus) {
    var grd1 = $("#grdSiteNoteViewMore").data("kendoGrid");
    grd1.dataSource.data(SiteNoteStatus);
    $("#hdnIsNotesEditable").val(SiteNoteStatus[0].isNotesEditable);

}

function EditSiteNote(SiteNoteId) {
    var parameters = { "SiteNoteId": SiteNoteId, "LocationId": $("#LocationList").data("kendoComboBox").value() };
    $.post(ViewEditSiteNoteUrl, parameters, EditSiteNoteCallback, "json");
}

function EditSiteNoteCallback(ResultSet) {
    fnshowViewEditNotes(ResultSet.Text, false);
    // This hidden field is added to check whether the site notes need to be added or updated.
    $("#hdnSiteNoteId").val(ResultSet.Id);
}

function ViewSiteNote(SiteNoteId) {
    var parameters = { "SiteNoteId": SiteNoteId, "LocationId": $("#LocationList").data("kendoComboBox").value() };
    $.post(ViewEditSiteNoteUrl, parameters, ViewSiteNoteCallback, "json");
}

function ViewSiteNoteCallback(ResultSet) {
    fnshowViewEditNotes(ResultSet.Text, true);
    // This hidden field is added to check whether the site notes need to be added or updated.
    $("#hdnSiteNoteId").val(ResultSet.Id);
}

function fnshowViewEditNotes(displayText, isReadOnly) {
    $("#SiteAddNotewindow").data("kendoWindow").center().open();
    $("#txtSiteAddNotes").val('');
    // To add existing notes to the label in pop up window            
    var existComments = displayText;
    $("#txtSiteAddNotes").val(existComments);
    if (isReadOnly == true) {
        $("#txtSiteAddNotes").attr('readonly', true);
    }
    else {
        $("#txtSiteAddNotes").attr('readonly', false);
    }
}

function grdSiteNotesChanged() {
    var selectedSiteInfo = this.select();
    GlobalSiteNoteId = this.dataItem(selectedSiteInfo).Id;
}

function onChange(e) {
    grid = e.sender;
    var currentDataItem = grid.dataItem(this.select());
    var wnd = $("#SiteNoteDetail").data("kendoWindow");
    wnd.content(currentDataItem.Text);
    wnd.open().center();
}

function fnSaveDefaults() {
    var parameters = { "SiteId": $("#LocationList").data("kendoComboBox").value(), "InternalName": "SITEINFORMATION", "ControlName": "LocationList" };
    $.post(siteSaveDefaultValue, parameters, null, "json");

    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}
function fnSiteInfoClearDefaults() {
    var parameters = { "InternalName": "SITEINFORMATION", "ControlName": "LocationList" };
    $.post(siteInfoClearDefaultsUrl, parameters, ClearSiteInfoDefaultValueCallBack, "json");

    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}
function ClearSiteInfoDefaultValueCallBack(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        fnShowAlertInfo(ResultSet);
    }
}

function GetSiteList() {
    $.post(siteInformationUrl, null, GetGetSiteListCallback, "json");
}

function GetGetSiteListCallback(ResultSet) {
    var cboLocationList = $("#LocationList").data("kendoComboBox");
    cboLocationList.dataSource.data(ResultSet);
    if (ResultSet != null && ResultSet.length > 0 && ResultSet[0].DefaultSelectedValue != undefined
            && ResultSet[0].DefaultSelectedValue != null
                && ResultSet[0].DefaultSelectedValue != "") {
        var selectedItem = ResultSet[0].DefaultSelectedValue;
        cboLocationList.value(selectedItem);
        loadSiteData(selectedItem);
    }
}

function fncloseImagepopup() {
    $("#Imgwindow").data("kendoWindow").close();
}
function btnOkSiteInfoClicked() {
    var SiteNoteText = $('#txtSiteAddNotes').val();
    if (SiteNoteText == 'undefined' || SiteNoteText == null || SiteNoteText == '') {
        alert('Please enter notes to be inserted.');
    }
    else {
        if ($('#hdnSiteNoteId').val() == '') {
            var dataItem = { "siteId": $('#LocationList').val(), "Notes": $('#txtSiteAddNotes').val() };
            $.post(siteAddNotesUrl, dataItem, GetNotesbySite, "json");
            $("#SiteAddNotewindow").data("kendoWindow").close();
        }
        else {
            var dataItem = { "siteId": $('#LocationList').val(), "Notes": $('#txtSiteAddNotes').val(), "SiteNoteId": $('#hdnSiteNoteId').val() };
            $.post(siteEditNotesUrl, dataItem, GetNotesbySite, "json");
            $("#SiteAddNotewindow").data("kendoWindow").close();
            $('#hdnSiteNoteId').val('');
        }
    }
}
function GetNotesbySite(ResultSet) {
    $("#txtSiteAddNotes").val('');
    $("#txtSiteComments").val(ResultSet);
    $("#SiteAddNotewindow").data("kendoWindow").close();

    //Load grid in Popup
    var grd1 = $("#grdSiteNoteViewMore").data("kendoGrid");
    grd1.dataSource.data(ResultSet);
    $("#hdnIsNotesEditable").val(ResultSet[0].isNotesEditable);
}
function btnCancelSiteInfoClicked() {
    $("#SiteAddNotewindow").data("kendoWindow").close();
}
function btnCancelViewNoteClicked() {
    $("#dvSiteNotes").css({ "display": "none" });
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function btnSearchSiteCancelClicked() {
    $("#wndSearchSiteInfoDetail").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}


function siteOnChange(e) {
    BlockElement("SiteInformation", 'Connecting...');

    var index = e.item.index();
    var test = this.dataItem(index).Id;
    loadSiteData(test);
}

function loadSiteData(id) {
    var dataItem = { "LocationId": id };
    $.post(siteGetLocationDetailByLocationIdUrl, dataItem, GetLocationDetailbyLocationCallback, "json");
}

function GetLocationDetailbyLocationCallback(ResultSet) {    
    $("#lblSitePortletLocation").val('');
    $("#lblSitePortletAddress").val('');
    $("#lblSitePortletLocContact").val('');
    $("#lblSiteContactEmail").val('');
    $("#lblSiteContactPhone").val('');
    $("#lblSiteDefaultDocument").val('');
    $("#txtSiteComments").val('');
    $('#imgsiteLogo').attr('src', '');
    $("#hdnsiteLogo").val('');
    $("#hdnUserRole").val('');
    $("#hdnSiteInfoContactEmail").val('');
    $("#lblSitePortletLocation").text(ResultSet.Location);
    $("#lblSitePortletAddress").text(ResultSet.Address);
    $("#lblSitePortletLocContact").text(ResultSet.LocationContact);
    if (ResultSet.ContactEmail.length > 15) {
        $("#lblSiteContactEmail").text(ResultSet.ContactEmail.substring(0, 15) + '...');
    }
    else {
        $("#lblSiteContactEmail").text(ResultSet.ContactEmail);
    }
    $("#hdnSiteInfoContactEmail").val(ResultSet.ContactEmail);
    $("#lblSiteContactPhone").text(ResultSet.ContactPersonPhone);
    $("#lblSiteDefaultDocument").text(ResultSet.DefaultDocument);
    $("#txtSiteComments").val(ResultSet.notes);
    if (ResultSet.siteImage != undefined && ResultSet.siteImage != null && ResultSet.siteImage != "") {
        $('#imgsiteLogo').attr('src', ResultSet.siteImage);
        $('#imgsiteLogo').attr('onclick', "fnshowImage();");
        $("#hdnsiteLogo").val(ResultSet.siteImage);
    }
    else {
        $('#imgsiteLogo').attr('src', siteLargeTransparentUrl);
        $('#imgsiteLogo').attr('onclick', "");
    }

    $("#lblSPVideoHealthCount").text(ResultSet.VideoHealthDevice);
    $("#lblSPIntrusionDevices").text(ResultSet.IntrusionDevice);
    $("#lblSPAccessDevices").text(ResultSet.AccessDevice);
    $("#hdnUserRole").val(ResultSet.CurrentUserRole);
    // Popup Grid
    var grd1 = $("#grdSiteNoteViewMore").data("kendoGrid");
    if (ResultSet.SiteNote.length == 1) {
        if (ResultSet.SiteNote[0].Id == 0) {
            grd1.dataSource.data([]);
        }
        else {
            grd1.dataSource.data(ResultSet.SiteNote);
        }
    }
    else {
        grd1.dataSource.data(ResultSet.SiteNote);
    }
    $("#hdnIsNotesEditable").val(ResultSet.SiteNote[0].isNotesEditable);
    UnBlockElement("SiteInformation");
}

function fnshowAddNotes() {
    if ($('#LocationList').val() != '') {
        $("#SiteAddNotewindow").data("kendoWindow").center().open();
        $("#txtSiteAddNotes").val('');
        // To add existing notes to the label in pop up window            
        var existComments = $("#txtSiteComments").val();
        $("#lblExistNotes").text(existComments);
        $("#txtSiteAddNotes").attr('readonly', false);
    }
    else {
        displaySiteErrorMessage("Please select site to add notes.");
    }
}

function fnshowDocument() {
    // Get Site Documents based on Site Id Selected
    var SiteId = $("#LocationList").data("kendoComboBox").value();
    var dataItem = { "SiteId": SiteId };
    if (dataItem.SiteId != null && dataItem.SiteId != undefined && dataItem.SiteId != '') {
        BlockElement("divSiteInfoContent", 'Connecting...');
        $.post(siteDocumentGetUrl, dataItem, GetSiteDocumentCallback, "json");
    }
    else {
        displaySiteErrorMessage("Please select site to add documents.");
    }
}

function GetSiteDocumentCallback(SiteDocumentResultSet) {
    UnBlockElement("divSiteInfoContent");
    if (SiteDocumentResultSet != null && SiteDocumentResultSet != undefined) {
        var grddocument = $("#grdSiteDocument").data("kendoGrid");
        // grddocument.dataSource.data(SiteDocumentResultSet);
        if (SiteDocumentResultSet.length == 1) {
            if (SiteDocumentResultSet[0].Id == 0) {
                grddocument.dataSource.data([]);
            }
            else {
                grddocument.dataSource.data(SiteDocumentResultSet);
            }
        }
        else {
            grddocument.dataSource.data(SiteDocumentResultSet);
        }
        $("#hdnIsDocumentEditable").val(SiteDocumentResultSet[0].isDocumentsEditable);
        $("#hdnIsDocumentDeletable").val(SiteDocumentResultSet[0].isDocumentsDeleteable);
        $("#hdnIsDocumentViewable").val(SiteDocumentResultSet[0].isDocumentsViewable);
        if ($('#hdnIsDocumentEditable').val() == 'true') {
            $("#dvUploadDocs").css({ "display": "block" });
        }
        else {
            $("#dvUploadDocs").css({ "display": "none" });
        }
        $("#my_overlay").css({
            "display": "block",
            "visibility": "visible"
        });
        centerViewPortDialog("#dvSiteDocument");

    }
}

function onDocumentUploadSuccess(e) {
    // Reload the Grid with newly uploaded File
    fnshowDocument();
}

function onDocumentUpload(e) {
    var grddocument = $("#grdSiteDocument").data("kendoGrid");
    if (grddocument.dataSource.total() > 9) {
        e.preventDefault();
        displaySiteErrorMessage("Only 10 documents can be uploaded");
    }
}

function onDocumnetUploadError(e) {
    displaySiteErrorMessage('Upload document Failed.');
}

function ViewSiteDocument(Id) {
    var dataItem = { "DocumentId": Id };
    if (dataItem.DocumentId != null && dataItem.DocumentId != undefined && dataItem.DocumentId != '') {
        $.post(GetsiteDocumentContentUrl, dataItem, GetSiteDocumentContentCallback, "json");
    }
}

function GetSiteDocumentContentCallback(ResultSet) {

}

function DeleteSiteDocument(Id) {
    var SiteId = $("#LocationList").data("kendoComboBox").value();
    var dataItem = { "SiteId": SiteId, "DocumentId": Id };
    if (dataItem.SiteId != null && dataItem.SiteId != undefined && dataItem.SiteId != '') {
        var wndDocs = $("#wndDocs").data("kendoWindow");
        wndDocs.content("<p style='height:100px'>Are you sure you want to delete the item?</p><p>" +
                "<input type='button' value='Ok' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-right:4px;' onclick='javascript:fndeleteDocs(" + SiteId + "," + Id + ");'>" +
                 "<input type='button' value='Cancel' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-left:5px;' onclick='ConfirmlnkCancelDocs()'></p>");
        wndDocs.center().open();
    }
    else {
        displaySiteErrorMessage("Please select site to delete a documents.");
    }
}

function fndeleteDocs(SiteId, Id) {
    var dataItem = { "SiteId": SiteId, "DocumentId": Id };
    if (dataItem.SiteId != null && dataItem.SiteId != undefined && dataItem.SiteId != '') {
        $.post(siteDocumentDeleteURL, dataItem, GetSiteDocumentCallback, "json");
        $("#wndDocs").data("kendoWindow").close();
    }
    else {
        displaySiteErrorMessage("Please select site to delete a documents.");
    }
}

function ConfirmlnkCancelDocs() {
    $("#wndDocs").data("kendoWindow").close();
}

function fnSetPrimary(Id) {
    $("#grdSiteDocument").on("change", "input.chkbxq", function (e) {
        var v = $(this).is(":checked");
        $("input.chkbxq", "#grdSiteDocument").prop("checked", false);
        $(this).prop("checked", v);
    });
    var dataItem = { "DocumentId": Id, "SiteId": $("#LocationList").data("kendoComboBox").value() };
    if (dataItem.SiteId != null && dataItem.SiteId != undefined && dataItem.SiteId != '') {
        $.post(siteDocumentUpdateURL, dataItem, GetSiteDocumentCallback, "json");
    }
}

function fnshowInventory() {
    var SiteId = $("#LocationList").data("kendoComboBox").value();
    if (SiteId != null && SiteId != undefined && SiteId != '') {
        var dataItem = { "SiteId": SiteId };
        if (dataItem.SiteId != null && dataItem.SiteId != undefined && dataItem.SiteId != '') {
            BlockElement("divSiteInfoContent", 'Connecting...');
            $.post(GetsiteInventoryUrl, dataItem, GetSiteInventoryCallback, "json");
        }
    }
    else {
        displaySiteErrorMessage("Please select site to view or edit inventory.");
    }
}

function GetSiteInventoryCallback(ResultSet) {
    UnBlockElement("divSiteInfoContent");
    var grdInventory = $("#grdCompanyInventory").data("kendoGrid");
    if (ResultSet != null && ResultSet.length == 1) {
        if (ResultSet[0].InventoryValue == 'No Records Found') {
            grdInventory.dataSource.data([]);
        }
        else {
            grdInventory.dataSource.data(ResultSet);
        }
    }
    else {
        grdInventory.dataSource.data(ResultSet);
    }
    $("#my_overlay").css({
        "display": "block",
        "visibility": "visible"
    });
    centerViewPortDialog("#dvSiteInventory");
}

function ViewSiteInventory(Id, IsEditable) {
    var grid = $("#grdCompanyInventory").data("kendoGrid");
    $("#SiteUDFDetails").data("kendoWindow").center().open();
    var row = grid.select();
    // SeletedCompanyInventoryIndex = row.index();
    var data = grid.dataItem(row);
    var siteKey = document.getElementById("txtEditKey");
    var siteValue = document.getElementById("txtEditValue");
    siteKey.value = data.InventoryKey;
    siteValue.value = data.InventoryValue;
    document.getElementById("txtEditKey").readOnly = true;
    document.getElementById("txtEditValue").readOnly = true;
    document.getElementById("btnSiteUDF").value = "OK";
    var OkClickedViewInv = document.getElementById("btnSiteUDF");
    OkClickedViewInv.onclick = fnSiteInventoryViewOkClicked;
}

function fnSiteInventoryViewOkClicked() {
    $("#SiteUDFDetails").data("kendoWindow").close();
}

function fnSiteInventoryViewEditClicked() {
    var SiteId = $("#LocationList").data("kendoComboBox").value();
    var grid = $("#grdCompanyInventory").data("kendoGrid");
    var row = grid.select();
    var data = grid.dataItem(row);
    var InveKey = document.getElementById('txtEditKey').value;
    var InveValue = document.getElementById('txtEditValue').value;
    if (InveKey != null && InveKey != undefined && InveKey != '' && InveValue != null && InveValue != undefined && InveValue != '') {
        var dataItem = { "CompanyInventoryId": intCompanyInventoryId, "InventoryKey": InveKey, "InventoryValue": InveValue, "SiteId": SiteId };
        if (dataItem.SiteId != null && dataItem.SiteId != undefined && dataItem.SiteId != '') {
            $.post(siteInventoryEditURL, dataItem, GetSiteInventoryCallback, "json");
        }
        $("#SiteUDFDetails").data("kendoWindow").close();
    }
    else {
        fnShowAlertInfo('Both UDF Key and Value are required fields. Please specify.');
    }

}


function EditSiteInventory(Id, IsEditable) {
    var grid = $("#grdCompanyInventory").data("kendoGrid");
    $("#SiteUDFDetails").data("kendoWindow").center().open();
    var row = grid.select();
    // SeletedCompanyInventoryIndex = row.index();
    var data = grid.dataItem(row);
    document.getElementById('txtEditKey').value = data.InventoryKey;
    document.getElementById('txtEditValue').value = data.InventoryValue;
    document.getElementById("txtEditKey").readOnly = true;
    document.getElementById("txtEditValue").readOnly = false;
    document.getElementById("btnSiteUDF").value = "Save";
    var EditClickedViewInv = document.getElementById("btnSiteUDF");
    intCompanyInventoryId = Id;
    EditClickedViewInv.onclick = fnSiteInventoryViewEditClicked;
}

function btnSiteInventoryokClick() {
    $("#dvSiteInventory").css({ "display": "none" });
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function fnUDFManagementClick() {
    var dataItem = { "CompanyId": $("#ExternalCompanyId").val() };
    if (dataItem.CompanyId != null && dataItem.CompanyId != undefined && dataItem.CompanyId != '') {
        $.post(siteInventoryDisplayURL, dataItem, siteInventoryDisplayCallback, "json");
    }
}

function siteInventoryDisplayCallback(Resultset) {
    var grd1 = $("#grdUDFManagement").data("kendoGrid");
    grd1.dataSource.data(Resultset);
    $("#Inventorywindow").data("kendoWindow").center().open();
}

function UDFManagementCreate() {
    var UDFgrd = $("#grdUDFManagement").data("kendoGrid");
    if (UDFgrd._data.length >= 10) {
        alert('Only 10 inventory items can be added.');
        return;
    }
    else {
        document.getElementById('txtKey').value = '';
        var wnd = $("#UDFDetails").data("kendoWindow");
        wnd.center().open();

    }

}

function fnAddUDFtoGrid() {
    var ExternalCompanyId = document.getElementById('ExternalCompanyId').value;
    var Key = document.getElementById('txtKey').value;
    //var grid = $("#grdUDFManagement").data("kendoGrid");
    //var datasource = grid.dataSource;
    //datasource.insert({ InventoryKey: Key, InventoryValue: Value });
    if (ExternalCompanyId != null && ExternalCompanyId != undefined && ExternalCompanyId != '' && ExternalCompanyId != 0) {
        if (Key != null && Key != undefined && Key != '') {
            var dataItem = { "ExternalCompanyId": ExternalCompanyId, "Key": Key };
            $.post(siteInventoryCreateURL, dataItem, siteInventoryDisplayCallback, "json");
            var wnd = $("#UDFDetails").data("kendoWindow");
            wnd.close();
        }
        else {
            alert('UDF Key is a required field. Please specify.');
        }
    }
    else {
        alert('Company Id is a required field. Please specify.');
    }
}

function fnEditUDFtoGrid() {
    var ExternalCompanyId = document.getElementById('ExternalCompanyId').value;
    var Key = document.getElementById('txtEditKey').value;
    var grid = $("#grdUDFManagement").data("kendoGrid");
    var datasource = grid.dataSource;
    var firstItem = datasource.data()[SeletedCompanyInventoryIndex];
    //firstItem.set('InventoryKey', Key);
    //firstItem.set('InventoryValue', Value);
    if (ExternalCompanyId != null && ExternalCompanyId != undefined && ExternalCompanyId != '' && ExternalCompanyId != 0) {
        if (Key != null && Key != undefined && Key != '') {
            var dataItem = { "ExternalCompanyId": ExternalCompanyId, "Key": Key, "InventoryId": firstItem.Id };
            $.post(siteInventoryEditURL, dataItem, siteInventoryDisplayCallback, "json");
            var wnd = $("#UDFDetailsEdit").data("kendoWindow");
            wnd.close();
        }
        else {
            alert('UDF Key is a required field. Please specify.');
        }
    }
    else {
        alert('Company Id is a required field. Please specify.');
    }
}

function btnInventoryClicked() {
    //    var gridData = $("#grdUDFManagement").data("kendoGrid").dataSource.data();
    //    // var dataItem = JSON.parse('{ UDFData: ' + gridData + ' }');
    //    var listItems = JSON.stringify(gridData);
    //    var dataItem = { UDFData: JSON.parse(listItems) };
    //    $.post(siteInventoryCreateURL, "[{\"InventoryKey\":\"sad\",\"InventoryValue\":\"dsa\"}]", siteInventoryDisplayCallback, "json");

    //    $.ajax({
    //        type: "POST"
    //        , url: "/Company/UDFMangementSave"
    //        , data: JSON.stringify({ UDFData: gridData })
    //        , contentType: "application/json"
    //    });
    var wnd = $("#Inventorywindow").data("kendoWindow");
    wnd.close();
}

function EditUDFDetails() {
    var grid = $("#grdUDFManagement").data("kendoGrid");
    var row = grid.select();
    SeletedCompanyInventoryIndex = row.index();
    var data = grid.dataItem(row);
    document.getElementById('txtEditKey').value = data.InventoryKey;
    var wnd = $("#UDFDetailsEdit").data("kendoWindow");
    wnd.center().open();
}

function DeleteUDFDetails(e) {
    var wnd = $("#wndDeleteInventory").data("kendoWindow");
    wnd.content("<p style='height:100px'>Are you sure you want to delete the item(s)?</p><p>" +
                "<input type='button' value='Ok' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-right:4px;' onclick='fndeleteInventoryDetails()'>" +
                 "<input type='button' value='Cancel' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-left:5px;' onclick='fnCancelInventoryDetails()'></p>");
    wnd.center().open();
}

function fndeleteInventoryDetails() {
    var ExternalCompanyId = document.getElementById('ExternalCompanyId').value;
    var gview = $("#grdUDFManagement").data("kendoGrid");
    var gridData = gview.dataSource.data();
    //Getting selected row
    var dataItem = gview.dataItem(gview.select());
    if (dataItem.Id != undefined) {
        var SelectedId = dataItem.Id;
        var dataItem = { "CompanyInventoryId": SelectedId, "CompanyExternalId": ExternalCompanyId };
        $.post(siteInventoryDeleteURL, dataItem, siteInventoryDisplayCallback, "json");
    }
    else {
        //Removing Selected row
        gview.dataSource.remove(dataItem);
        //Removing row using index number
        // gview.dataSource.remove(0); // 0 is row index
    }
    var wnd = $("#wndDeleteInventory").data("kendoWindow");
    wnd.close();
}

function fnCancelInventoryDetails() {
    var wnd = $("#wndDeleteInventory").data("kendoWindow");
    wnd.close();
}

function btnCancelViewDocumentClicked() {
    $("#dvSiteDocument").css({ "display": "none" });
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}

function btnAddDocumentClicked() {

}

function onDocumentSelect(e) {
    var grid = $('#grdSiteDocument').data().kendoGrid;
    var Count = 0;
    $.each(grid.dataSource.view(), function () {
        Count += 1;
        if (Count > 10) {
            displaySiteErrorMessage("Only 10 documents can be uploaded.");
            e.preventDefault();
            return;
        }
    });
    $.each(e.files, function (index, value) {
        if (value.size > 2048000) {
            displaySiteErrorMessage("File size cannot be greater than 2 MB.");
            e.preventDefault();
            return;
        }
        if (value.extension.toLowerCase() != '.pdf') {
            displaySiteErrorMessage("Only PDF files can be uploaded.");
            e.preventDefault();
            return;
        }
    });

}
function fnshowImage() {
    $("#Imgwindow").data("kendoWindow").center().open();
    $('#imgpopupSiteLogo').attr('src', $("#hdnsiteLogo").val());
}
function displaySiteErrorMessage(msg) {
    alert(msg);
}

function ShowSiteInformationPortletDes() {
    $("#SiteInfoDeswindow").data("kendoWindow").open().center();
}

function fnCloseSiteInfoDeswindow() {
    $("#SiteInfoDeswindow").data("kendoWindow").close();
}

function fnCloseSiteInformation() {
    var mySplitResultset = {
        'input': 'SiteInformation'
    };
    $.post(siteClosePortlet, mySplitResultset, mySplitResultset, "json");

    $('#SiteInformation').hide();
    getWidthofColumn();
    return false;
}

function fntoggleSiteInformation() {
    $('#divSiteInfoContent').toggle();
}


function openSite() {
    if ($('#mainBody .k-animation-container #LocationList-list').length > 0) {
        if ($('#mainBody > #LocationList-list').length > 0) {
            $('#LocationList-list').remove();
        }
    }
    $('#LocationList-list').prepend("<div id='DDsiteheader'><div id='rowcell'>Name</div><div id='rowcell'>Address</div><div id='rowcell'>City</div><div id='rowcell'>State</div><div id='rowcell' style='border-right:0;'>Zip</div></div>");
}
function closeSite() {
    $('#DDsiteheader').remove();
}

function fnshowViewMore() {
    if ($('#LocationList').val() != '') {
        BlockElement("divSiteInfoContent", 'Connecting...');
        $("#my_overlay").css({
            "display": "block",
            "visibility": "visible"
        });
        centerViewPortDialog("#dvSiteNotes");
        if ($('#hdnIsNotesEditable').val() == 'true') {
            $("#btnInfoPopupAddNotes").css({
                "display": "block",
                "visibility": "visible"
            });
        }
        else {
            $("#btnInfoPopupAddNotes").css({
                "display": "none",
                "visibility": "hidden"
            });
        }
        UnBlockElement("divSiteInfoContent");
    }
    else {
        displaySiteErrorMessage("Please select site to view notes.");
    }
}
function fnAddAccNumber() {

    //    if ($('#LocationList').val() != '') {
    //        BlockElement("divSiteInfoContent", 'Connecting...');
    //        $("#my_overlay").css({
    //            "display": "block",
    //            "visibility": "visible"
    //        });
    //        centerViewPortDialog("#dvSiteNotes");
    //        if ($('#hdnIsNotesEditable').val() == 'true') {
    //            $("#btnInfoPopupAddNotes").css({
    //                "display": "block",
    //                "visibility": "visible"
    //            });
    //        }
    //        else {
    //            $("#btnInfoPopupAddNotes").css({
    //                "display": "none",
    //                "visibility": "hidden"
    //            });
    //        }
    //        UnBlockElement("divSiteInfoContent");
    //    }
    //    else {
    //        displaySiteErrorMessage("Please select site to view notes.");
    //    }
    $("#my_overlay").css({
        "display": "block",
        "visibility": "visible"
    });


    centerViewPortDialog1("#dvSiteAccountNumber");
    var parameters = { "siteId": $('#SiteId').val() };
    $.post(GetsiteAccNumberUrl, parameters, RemoveSiteAccNumberCallback, "json");
}
function GetSiteAccNumberCallback(ResultSet) {
    //Load grid in Popup
    var grd1 = $("#grdSiteAccNumber").data("kendoGrid");
    grd1.dataSource.data(ResultSet);
}
function ViewSiteAccNumber(SiteAccNumberId) {
    //    var parameters = { "SiteNoteId": SiteNoteId, "LocationId": $("#LocationList").data("kendoComboBox").value() };
    //    $.post(ViewEditSiteNoteUrl, parameters, ViewSiteNoteCallback, "json");

    var parameters = { "SiteAccNumberId": SiteAccNumberId, "siteId": $('#SiteId').val() };
    $.post(ViewEditSiteAccNumUrl, parameters, ViewSiteAccNumberCallback, "json");
}
function ViewSiteAccNumberCallback(ResultSet) {
    fnshowViewEditAccNumber(ResultSet.AccountNumber, ResultSet.IsAssociatedWithFA, true);
    // This hidden field is added to check whether the site notes need to be added or updated.
    //$("#hdnSiteNoteId").val(ResultSet.Id);
    $("#hdnSiteAccNumId").val(ResultSet.Id);


}
function fnshowViewEditAccNumber(displayText, isAssiciated, isReadOnly) {
    $("#SiteAddAccNumberwindow").data("kendoWindow").center().open();
    $("#txtSiteAccNumber").val('');
    document.getElementById('chk_isAssociated').checked = false;

    // To add existing notes to the label in pop up window            
    var existComments = displayText;
    $("#txtSiteAccNumber").val(existComments);
    document.getElementById('chk_isAssociated').checked = isAssiciated;

    if (isReadOnly == true) {
        $("#txtSiteAccNumber").attr('readonly', true);
        $("#chk_isAssociated").attr("disabled", true);
    }
    else {
        $("#txtSiteAccNumber").attr('readonly', false);
        $("#chk_isAssociated").attr("disabled", false);
    }
}

function EditSiteAccNumber(SiteAccNumberId) {
    var parameters = { "SiteAccNumberId": SiteAccNumberId, "siteId": $('#SiteId').val() };
    $.post(ViewEditSiteAccNumUrl, parameters, EditSiteAccNumberCallback, "json");
}

function EditSiteAccNumberCallback(ResultSet) {
    //    fnshowViewEditNotes(ResultSet.Text, false);
    //    // This hidden field is added to check whether the site notes need to be added or updated.
    //    $("#hdnSiteNoteId").val(ResultSet.Id);

    fnshowViewEditAccNumber(ResultSet.AccountNumber, ResultSet.IsAssociatedWithFA, false);
    // This hidden field is added to check whether the site notes need to be added or updated.
    $("#hdnSiteAccNumId").val(ResultSet.Id);
}

function DeleteSiteAccNumber(SiteId) {
    var wnd = $("#DeleteAccountNumber").data("kendoWindow");
    wnd.content("<p style='height:100px'>Are you sure you want to delete this item?</p><p>" +
            "<input type='button' value='Ok' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-right:4px;' onclick='javascript:ConfirmDeleteAccNumberOk(" + SiteId + ");'>" +
                "<input type='button' value='Cancel' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-left:5px;' onclick='ConfirmDeleteAccNumberCancel()'></p>");
    wnd.center().open();
    //    }
}

function ConfirmDeleteAccNumberCancel() {
    $("#DeleteAccountNumber").data("kendoWindow").close();
}
function ConfirmDeleteAccNumberOk(SiteId) {
    //    $("#DeleteNote").data("kendoWindow").close();
    //    var parameters = { "SiteNoteId": SiteId, "LocationId": $("#LocationList").data("kendoComboBox").value() };
    //    $.post(siteRemoveSiteNote, parameters, RemoveSiteNoteCallback, "json");

    $("#DeleteAccountNumber").data("kendoWindow").close();
    var parameters = { "SiteAccNumberId": SiteId, "siteId": $('#SiteId').val() };
    $.post(siteRemoveSiteAccNum, parameters, RemoveSiteAccNumberCallback, "json");
}

function RemoveSiteAccNumberCallback(SiteNoteStatus) {
    //    var grd1 = $("#grdSiteNoteViewMore").data("kendoGrid");
    //    grd1.dataSource.data(SiteNoteStatus);
    //$("#hdnIsNotesEditable").val(SiteNoteStatus[0].isNotesEditable);

    var grd1 = $("#grdSiteAccNumber").data("kendoGrid");
    grd1.dataSource.data(SiteNoteStatus);
}

function btnCancelViewAccNumberClicked() {
    $("#dvSiteAccountNumber").css({ "display": "none" });
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function fnshowAddAccountNumber() {
    // if ($('#LocationList').val() != '') {
    $("#SiteAddAccNumberwindow").data("kendoWindow").center().open();
    $("#txtSiteAccNumber").val('');
    document.getElementById('chk_isAssociated').checked = false;
    //$("#lblExistNotes").text(existComments);
    $("#txtSiteAccNumber").attr('readonly', false);
}
function btnCancelSiteAccNumberClicked() {
    $("#SiteAddAccNumberwindow").data("kendoWindow").close();
}
function btnOkSiteAccNumClicked() {
    var SiteAccNumText = $('#txtSiteAccNumber').val();
    if (SiteAccNumText == 'undefined' || SiteAccNumText == null || SiteAccNumText == '') {
        alert('Please enter account number to be inserted.');
    }
    else {
        if ($('#hdnSiteAccNumId').val() == '') {
            var isAssociated = document.getElementById('chk_isAssociated').checked;
            var dataItem = { "siteId": $('#SiteId').val(), "accNumber": $('#txtSiteAccNumber').val(), "isAssociated": isAssociated };
            $.post(siteAddAccNumUrl, dataItem, GetAccNumberbySite, "json");
            $("#SiteAddAccNumberwindow").data("kendoWindow").close();
        }
        else {
            var isAssociated = document.getElementById('chk_isAssociated').checked;
            var dataItem = { "siteId": $('#SiteId').val(), "accNumber": $('#txtSiteAccNumber').val(), "SiteAccountId": $('#hdnSiteAccNumId').val(), "isAssociated": isAssociated };
            $.post(siteEditAccNumUrl, dataItem, GetAccNumberbySite, "json");
            $("#SiteAddAccNumberwindow").data("kendoWindow").close();
            $('#hdnSiteAccNumId').val('');
            $('#hdnisAssociated').val('');
        }
    }
    $("#SiteAddAccNumberwindow").data("kendoWindow").close();
}

function GetAccNumberbySite(ResultSet) {
    $("#txtSiteAccNumber").val('');
    $("#SiteAddAccNumberwindow").data("kendoWindow").close();

    //Load grid in Popup
    var grd1 = $("#grdSiteAccNumber").data("kendoGrid");
    grd1.dataSource.data(ResultSet);
}

function fnOpenSiteInfoDetails() {
    $.post(siteGetAllSiteDetailsforSearch, null, GetSiteInforSearchCallback, "json");
}

function GetSiteInforSearchCallback(ResultSet) {
    var grdSearchSiteInfoDetails = $("#grdSearchSiteInfoDetails").data("kendoGrid");
    grdSearchSiteInfoDetails.dataSource.data(ResultSet);
    centerViewPortDialog("#wndSearchSiteInfoDetail");
}

function grdSearchSiteChanged() {
    var selectedSiteInfo = this.select();
    var siteId = this.dataItem(selectedSiteInfo).Id;
    var siteName = this.dataItem(selectedSiteInfo).Name;
    var cboSearchLocationList = $("#LocationList").data("kendoComboBox");
    cboSearchLocationList.value(siteName);
    var SelectedSiteInfo = siteId;
    loadSiteData(SelectedSiteInfo);
    $("#wndSearchSiteInfoDetail").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}

// Site Map

function OpenSiteMap(id) {
    if (id != undefined && id != null) {
        // $("#imgSieMapwindow").data("kendoWindow").center().open();
        /*
        $("#wndSiteMapInfoDetail").css({ "display": "block" });

        $("#my_overlay").css({
        "display": "block",
        "visibility": "none"
        });
        centerViewPortDialog("#wndSiteMapInfoDetail");
        */

        BlockElement("dvSiteMap", 'Connecting...');
        var data = { siteId: id };
        $.post(siteMapGetSiteImageUrl, data, "json")
            .success(function (response) {
                if (response != undefined && response != "" && response != null) {
                    $("#imgSiteMapImagePlaceHolder").attr("src", response);
                }
                else {
                    $("#imgSiteMapImagePlaceHolder").attr("src", siteMapNotAvailableImageUrl);
                }
                // centerViewPortDialog("#wndSiteMapInfoDetail");
                // Get Details of the Documents
                $.post(siteMapDocumentGetUrl, data, GetSiteMapDocumentCallback, "json");
            })
            .fail(function (response) {
                $("#imgSiteMapImagePlaceHolder").attr("src", siteMapNotAvailableImageUrl);
            });
        // Get Details of the Documents
        //$.post(siteMapDocumentGetUrl, data, GetSiteMapDocumentCallback, "json");
    }
}


function GetSiteMapDocumentCallback(SiteDocumentResultSet) {
    if (SiteDocumentResultSet != null && SiteDocumentResultSet != undefined) {
        var grddocument = $("#grdSiteMapDocument").data("kendoGrid");
        // grddocument.dataSource.data(SiteDocumentResultSet);
        if (SiteDocumentResultSet.length == 1) {
            if (SiteDocumentResultSet[0].Id == 0) {
                grddocument.dataSource.data([]);
            }
            else {
                grddocument.dataSource.data(SiteDocumentResultSet);
            }
        }
        else {
            grddocument.dataSource.data(SiteDocumentResultSet);
        }

        UnBlockElement("dvSiteMap");
        $("#my_overlay").css({
            "display": "block",
            "visibility": "visible"
        });
        centerViewPortDialog("#wndSiteMapInfoDetail");
        $("#wndSiteMapInfoDetail").css({ "display": "block" });
    }
}

function fncloseSiteMapPopup() {
    // $("#imgSieMapwindow").data("kendoWindow").close();
    $("#wndSiteMapInfoDetail").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
    $("#imgSiteMapImagePlaceHolder").attr("src", "");
}

function fnCloseSiteMap() {
    var mySplitResultset = {
        'input': 'SiteMap'
    };
    $.post(siteMapClosePortletUrl, mySplitResultset, mySplitResultset, "json");
    $('#dvSiteMap').hide();
    getWidthofColumn();
    return false;
}

function fntoggleSiteMap() {
    $('#divSiteMapContent').toggle();
}

function initialize() {
    var myOptions = {
        //Setting zoom level for the map.
        zoom: 7,
        //Google map object initilization and setting default location on the map
        center: new google.maps.LatLng(36.1700, -119.7462),
        disableDefaultUI: false,
        //Setting google map type in the view.
        mapTypeId: google.maps.MapTypeId.ROADMAP
        //Othere types of maps - SATELLITE, ROADMAP,HYBRID,TERRAIN
    };
    //Rendring the google map in the UI
    map = new google.maps.Map(document.getElementById("dvSiteGmap"), myOptions);
    var weatherLayer = new google.maps.weather.WeatherLayer({
        temperatureUnits: google.maps.weather.TemperatureUnit.FAHRENHEIT
    });
    weatherLayer.setMap(map);
}

function populateMarkers(data) {

    if (map == undefined) {
        initialize();
    }
    var objSites = eval(data);

    for (var count = 0; count < objSites.length; count++) {
        if (objSites[count].Latitude != null) {
            var marker = createMarker(objSites[count].Id, objSites[count].Location, objSites[count].Address, objSites[count].LocationContact,
                                      objSites[count].ContactEmail, objSites[count].ContactPhone, objSites[count].TotalDevicesCount,
                                      objSites[count].DVRDevicesCount, objSites[count].AccessDevicesCount, objSites[count].IntrusionDevicesCount,
                                      objSites[count].SiteImage, objSites[count].Latitude, objSites[count].Longitude)
        }
    }
    if (objSites.length > 0) {
        if (objSites[objSites.length - 1].SessionLatitude != null && objSites[objSites.length - 1].SessionLongitude != null) {

            $("#dvSiteId").text(objSites[objSites.length - 1].SessionId);
            $("#dvLocation").text(objSites[objSites.length - 1].SessionLocation);
            $("#dvAddress").text(objSites[objSites.length - 1].SessionAddress);
            $("#dvLContact").text(objSites[objSites.length - 1].SessionLocationContact);
            $('#lblContEmail').attr('href', 'mailto:' + objSites[objSites.length - 1].SessionContactEmail);
            $('#hdnContactEmail').val(objSites[objSites.length - 1].SessionContactEmail);
            if (objSites[objSites.length - 1].SessionContactEmail.length > 50) {
                $("#lblContEmail").text(objSites[objSites.length - 1].SessionContactEmail.substring(0, 50) + '...');
            }
            else {
                $("#lblContEmail").text(objSites[objSites.length - 1].SessionContactEmail);
            }
            $("#lblContPhone").text(objSites[objSites.length - 1].SessionContactPhone);

            $("#lblVHDCount").text(objSites[objSites.length - 1].SessionDVRCount);
            $("#lblAcsDvsCount").text(objSites[objSites.length - 1].SessionAccessCount);
            $("#lblIntrnDvsCount").text(objSites[objSites.length - 1].SessionIntrusionCount);
            $('#lnkSitemap').attr('onclick', "OpenSiteMap(" + objSites[objSites.length - 1].SessionId + "); return false;");
            var siteLatlng = new google.maps.LatLng(objSites[objSites.length - 1].SessionLatitude, objSites[objSites.length - 1].SessionLongitude);
            map.setCenter(siteLatlng);

            // Function to make a API call to find whether any whether alerts are present
            var addrss = objSites[objSites.length - 1].SessionAddress.split(',');
            SelectedState = addrss[2];
            SelectedCity = addrss[1];
            var SelectedStateandCity = {
                'State': addrss[2],
                'City': addrss[1],
                'Latitude': objSites[objSites.length - 1].SessionLatitude,
                'Longitude': objSites[objSites.length - 1].SessionLongitude,
                'Location': objSites[objSites.length - 1].SessionLocation,
                'Address': objSites[objSites.length - 1].SessionAddress,
                'LocationContact': objSites[objSites.length - 1].SessionLocationContact,
                'ContactEmail': objSites[objSites.length - 1].SessionContactEmail,
                'ContactPhone': objSites[objSites.length - 1].SessionContactPhone,
                'DVRDevicesCount': objSites[objSites.length - 1].SessionDVRCount,
                'AccessDevicesCount': objSites[objSites.length - 1].SessionAccessCount,
                'IntrusionDevicesCount': objSites[objSites.length - 1].SessionIntrusionCount,
                'Id': objSites[objSites.length - 1].SessionId

            };

            $.post(SiteWeatherAlertURL, SelectedStateandCity, WeatherAlertCallBack, "json");
            BlockElement("dvSiteMap", 'Connecting...');
        }
        else {
            for (i = 0; i < objSites.length; i++) {
                if (objSites[i].Id == objSites[objSites.length - 1].DefaultSiteLocation) {
                    defaultSiteIndex = i + 1;
                }
            }

            if (defaultSiteIndex > 0) {
                $("#dvSiteId").text(objSites[defaultSiteIndex - 1].Id);
                $("#dvLocation").text(objSites[defaultSiteIndex - 1].Location);
                $("#dvAddress").text(objSites[defaultSiteIndex - 1].Address);
                $("#dvLContact").text(objSites[defaultSiteIndex - 1].LocationContact);
                $('#lblContEmail').attr('href', 'mailto:' + objSites[defaultSiteIndex - 1].ContactEmail);
                $('#hdnContactEmail').val(objSites[defaultSiteIndex - 1].ContactEmail);
                if (objSites[defaultSiteIndex - 1].ContactEmail.length > 50) {
                    $("#lblContEmail").text(objSites[defaultSiteIndex - 1].ContactEmail.substring(0, 50) + '...');
                }
                else {
                    $("#lblContEmail").text(objSites[defaultSiteIndex - 1].ContactEmail);
                }
                $("#lblContPhone").text(objSites[defaultSiteIndex - 1].ContactPhone);

                $("#lblVHDCount").text(objSites[defaultSiteIndex - 1].DVRDevicesCount);
                $("#lblAcsDvsCount").text(objSites[defaultSiteIndex - 1].AccessDevicesCount);
                $("#lblIntrnDvsCount").text(objSites[defaultSiteIndex - 1].IntrusionDevicesCount);
                $('#lnkSitemap').attr('onclick', "OpenSiteMap(" + objSites[defaultSiteIndex - 1].Id + "); return false;");

                // Function to make a API call to find whether any whether alerts are present
                var addrss = objSites[defaultSiteIndex - 1].Address.split(',');
                SelectedState = addrss[2];
                SelectedCity = addrss[1];
                var SelectedStateandCity = {
                    'State': addrss[2],
                    'City': addrss[1],
                    'Latitude': objSites[defaultSiteIndex - 1].Latitude,
                    'Longitude': objSites[defaultSiteIndex - 1].Longitude,
                    'Location': objSites[defaultSiteIndex - 1].Location,
                    'Address': objSites[defaultSiteIndex - 1].Address,
                    'LocationContact': objSites[defaultSiteIndex - 1].LocationContact,
                    'ContactEmail': objSites[defaultSiteIndex - 1].ContactEmail,
                    'ContactPhone': objSites[defaultSiteIndex - 1].ContactPhone,
                    'DVRDevicesCount': objSites[defaultSiteIndex - 1].DVRDevicesCount,
                    'AccessDevicesCount': objSites[defaultSiteIndex - 1].AccessDevicesCount,
                    'IntrusionDevicesCount': objSites[defaultSiteIndex - 1].IntrusionDevicesCount,
                    'Id': objSites[defaultSiteIndex - 1].Id
                };
            }

            $.post(SiteWeatherAlertURL, SelectedStateandCity, WeatherAlertCallBack, "json");
            UnBlockElement("dvSiteMap");
        }

    }

}

/*
Marker will be created on the google with the site information overlay.
Location - Location of the site
Address - Site Address
LContact - Location Contact person name
*/
function createMarker(Id, Location, Address, LocationContact, ContactEmail, ContactPhone,
                TotalDevicesCount, DVRDevicesCount, AccessDevicesCount, IntrusionDevicesCount, SiteImage, Latitude, Longitude) {
    infowindow = new google.maps.InfoWindow();
    var markerImage = "../Content/images/red-pushpin.png";
    var siteLatlng = new google.maps.LatLng(Latitude, Longitude);
    var tempMarker = new google.maps.Marker({
        position: siteLatlng,
        map: map,
        icon: markerImage
    });
    map.setCenter(siteLatlng);
    google.maps.event.addListener(tempMarker, "click", function () {
        $("#dvSiteId").text(Id);
        $("#dvLocation").text(Location);
        $("#dvAddress").text(Address);
        $("#dvLContact").text(LocationContact);

        // $("#lblContEmail").text(ContactEmail);
        $('#hdnContactEmail').val(ContactEmail);
        $('#lblContEmail').attr('href', 'mailto:' + ContactEmail);
        if (ContactEmail.length > 50) {
            $("#lblContEmail").text(ContactEmail.substring(0, 50) + '...');
        }
        else {
            $("#lblContEmail").text(ContactEmail);
        }

        $("#lblContPhone").text(ContactPhone);

        $("#lblVHDCount").text(DVRDevicesCount);
        $("#lblAcsDvsCount").text(AccessDevicesCount);
        $("#lblIntrnDvsCount").text(IntrusionDevicesCount);
        $('#lnkSitemap').attr('onclick', "OpenSiteMap(" + Id + "); return false;");
        // Function to make a API call to find whether any whether alerts are present
        var addrss = Address.split(',');
        SelectedState = addrss[2];
        SelectedCity = addrss[1];
        var SelectedStateandCity = {
            'State': addrss[2],
            'City': addrss[1],
            'Latitude': Latitude,
            'Longitude': Longitude,
            'Location': Location,
            'Address': Address,
            'LocationContact': LocationContact,
            'ContactEmail': ContactEmail,
            'ContactPhone': ContactPhone,
            'DVRDevicesCount': DVRDevicesCount,
            'AccessDevicesCount': AccessDevicesCount,
            'IntrusionDevicesCount': IntrusionDevicesCount,
            'Id': Id

        };


        //            infowindow.setContent('<body><table><tr><td style="color:black;">Link Utilization: ' + 'Haiii' + '%</td></tr><tr><td style="color:black;">Current Capacity: ' + 'Haiiiiiii' + '% </td></tr></table></body>');
        //            if (infowindow)
        //                infowindow.close();
        //            infowindow.setPosition(siteLatlng);
        //            infowindow.open(map);
        $.post(SiteWeatherAlertURL, SelectedStateandCity, WeatherAlertCallBack, "json");
        BlockElement("dvSiteMap", 'Connecting...');
    });
}

function WeatherAlertCallBack(WeatherresultSet) {
    UnBlockElement("dvSiteMap");
    if (WeatherresultSet != null) {
        if (WeatherresultSet.length > 0) {
            WeatherAlertMessage = WeatherresultSet;
            document.getElementById('lnkWeatherAlert').style.display = "block";
        }
        else {
            document.getElementById('lnkWeatherAlert').style.display = "none";
            WeatherAlertMessage = '';
        }
    }
    else {
        document.getElementById('lnkWeatherAlert').style.display = "none";
        WeatherAlertMessage = '';
    }
}

function fnWeatherAlertDisplay() {
    document.getElementById('dvWeatherAlerts').style.display = "block";
    var grid = $("#grdWeatherAlert").data("kendoGrid");
    grid.dataSource.data(WeatherAlertMessage);
    $("#my_overlay").css({
        "display": "block",
        "visibility": "visible"
    });
    centerViewPortDialogBig("#dvWeatherAlerts");
}

function btnCancelWeatherAlertClicked() {
    $("#dvWeatherAlerts").css({ "display": "none" });
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}


function fnSiteMapRefresh() {
    $.post(SiteMapRefreshURL, null, SiteMaprefreshCallBack, "json");
}

function SiteMaprefreshCallBack(SiteMapResultSet) {
    populateMarkers(SiteMapResultSet)
    if ((SelectedState != null || SelectedState != undefined || SelectedState != '') && (SelectedCity != null || SelectedCity != undefined || SelectedCity != '')) {
        var SelectedStateandCity = {
            'State': SelectedState,
            'City': SelectedCity
        };
        $.post(SiteWeatherAlertURL, SelectedStateandCity, WeatherAlertCallBack, "json");
        BlockElement("dvSiteMap", 'Connecting...');
    }
}

function fnSiteMapMoreInfo() {
    location.href = siteMapEditUri;
}

function ShowSiteMapDesc() {
    $("#SiteMapDescription").data("kendoWindow").center().open();
}

// Access View

function fnCloseAccessView() {
    var mySplitResultset = {
        'input': 'ACCESSCONTROL'
    };
    $.post(accessClosePortletUrl, mySplitResultset, mySplitResultset, "json");

    $('#dvAccessViewPortlet').hide();
    getWidthofColumn();
    return false;
}

function fntoggleAccessView() {
    $('#dvAccessViewContent').toggle();
}
function showMyOverlay() {
    var deviceID = $("#AccessViewDeviceList").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device.");
    } else {
        $("#dvErrCardholderAdd ul").empty();
        centerViewPortDialog("#dvAddCardHolder");
        document.getElementById('my_overlay').style.visibility = 'visible';
        document.getElementById('dvAddCardHolder').style.visibility = 'visible';

        $('#txtCardholderFirstName').val('');
        $('#txtCardholderLastName').val('');
        $('#txtCardholderNumber').val('');
        $('#txtCardholderPin').val('');

        var parameters = { "deviceId": deviceID };
        BlockElement("dvCardholderAddContent", 'Connecting...');
        $.post(accessGetGroupListUrl, parameters, GetAccessGroupListCallback, "json");
    }
}
function GetAccessGroupListCallback(ResultSet) {    
    UnBlockElement("dvCardholderAddContent");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboAddAccessGroupList = $("#cboCHAddAccessGroup").data("kendoComboBox");
            cboAddAccessGroupList.dataSource.data(ResultSet);
        }
    }
}
function hideMyOverlay() {
    document.getElementById('my_overlay').style.visibility = 'hidden';
    document.getElementById('dvAddCardHolder').style.visibility = 'hidden';
}

function showModifyCardHolderLookupFields() {
    var deviceID = $("#AccessViewDeviceList").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device.");
    } else {
        $("#dvErrCardModify ul").empty();
        centerViewPortDialog("#dvModifyCardHolder");
        $('#txtModifyFirstName').val('');
        $('#txtModifyLastName').val('');
        $('#txtModifyCardNum').val('');

    }
}

function GetAccessGroupListCallback_Modify2(ResultSet) {
    UnBlockElement("dvCardholderModify2Content");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboModifyAccessGroupList = $("#cboCHModifyAccessGroup").data("kendoComboBox");
            cboModifyAccessGroupList.dataSource.data(ResultSet);
            var cboCHModifyAccessGroup = $("#cboCHModifyAccessGroup").data("kendoComboBox");
            cboCHModifyAccessGroup.value(CHListaccessGroupId);
        }
    }
}
function GetDMPXRAccessGroupListCallback_Modify2(ResultSet) {
    UnBlockElement("dvDMPXRCardholderModify2Content");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboDMPXRModifyAccessGroupList = $("#cboDMPXRCHModifyAccessGroup").data("kendoComboBox");
            cboDMPXRModifyAccessGroupList.dataSource.data(ResultSet);
            var cboDMPXRCHModifyAccessGroup = $("#cboDMPXRCHModifyAccessGroup").data("kendoComboBox");
            cboDMPXRCHModifyAccessGroup.value(CHListaccessGroupId);
        }
    }
}
function DeleteCardHolder() {
    var deviceID = $("#AccessViewDeviceList").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device.");
    }
    else {
        $("#dvErrCardholderDelete ul").empty();
        centerViewPortDialog("#dvDeleteCrdHldr");
        $('#txtDeleteFirstName').val('');
        $('#txtDeleteLastName').val('');
        $('#txtDeleteCardNumber').val('');
    }
}

function fnShowAddAccessGroup() {
    var deviceID = $("#AccessViewDeviceList").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device.");
    }
    else {
        $("#dvErrAGAdd ul").empty();
        centerViewPortDialog("#dvAddAccessTimePeriods");
        uncheckAllByParent('tblReaders', false);
        uncheckAllByParent('tblTimePeriods', false);
        $('#txtAddAGName').val('');
        $('#txtAddAGDescription').val('');
        document.getElementById('dvAGTimePeriods').style.display = 'none';
        document.getElementById('dvTImeAddbtn').style.display = 'block';
        $("#lblAGheader").text('Step 1: Enter General Group Details');
    }
}

function fnShowModifyAccessGroup() {

    var deviceID = $("#AccessViewDeviceList").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device.");
    } else {

        $("#dvAGModifyValidations ul").empty();
        centerViewPortDialog("#dvModifyAccessGroups");
        $('#cboCHModifyAG').val('');
        document.getElementById('dvModifyAGContent').style.display = 'none';
        document.getElementById('dvAddAGbtn').style.display = 'none';
        document.getElementById('dvSaveAGbtn').style.display = 'none';
        document.getElementById('dvcboAccessGroup').style.display = 'block';
        document.getElementById('dvMainbtns').style.display = 'block';
        $('#cboCHModifyAG').val('');
        var parameters = { "deviceId": deviceID };
        BlockElement("dvCardholderModifyContent", 'Connecting...');
        $.post(accessGetGroupListUrl, parameters, GetAccessGroupListCallback_Modify, "json");
    }
}

function GetAccessGroupListCallback_Modify(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboModifyAccessGroupList = $("#cboCHModifyAG").data("kendoComboBox");
            cboModifyAccessGroupList.dataSource.data(ResultSet);
        }
    }
    UnBlockElement("dvCardholderModifyContent");
}

function fnShowDeleteAccessGroup() {
    var deviceID = $("#AccessViewDeviceList").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device.");
    } else {
        $("#DeleteGroupListValidations ul").empty();
        centerViewPortDialog("#dvDeleteAccessGroups");
        $('#cboCHDeleteAccessGroup').val('');
        var parameters = { "deviceId": deviceID };
        BlockElement("dvDeleteAGContent", 'Connecting...');
        $.post(accessGetGroupListUrl, parameters, GetAccessGroupListCallback_Delete, "json");
    }
}

function GetAccessGroupListCallback_Delete(ResultSet) {
    UnBlockElement("dvDeleteAGContent");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboDeleteAccessGroupList = $("#cboCHDeleteAccessGroup").data("kendoComboBox");
            cboDeleteAccessGroupList.dataSource.data(ResultSet);

        }
    }
}

function fnShowMomentaryUnlock() {
    $("#dvErrMomentaryOpenDoor ul").empty();
    var parameters = { "deviceId": $("#AccessViewDeviceList").val() };
    BlockElement("dvAccessViewContent", 'Connecting...');
    $.post(accessGetReadersListUrl, parameters, GetReadersListCallback_Momentary, "json");
}
function GetReadersListCallback_Momentary(ResultSet) {
    UnBlockElement("dvAccessViewContent");
    var readersID;
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            for (i = 0; i < ResultSet.length; i++) {
                if (ResultSet[i].Name == $("#hdnAccessDoorName").val()) {
                    readersID = ResultSet[i].Id;
                }
            }
        }
    }
    var dataItem = { "readerId": readersID, "deviceId": $("#AccessViewDeviceList").val() };
    $.post(accessAccessMomentaryOpenDoorUrl, dataItem, GetMomentaryUnlockCallback, "json");
}

function GetMomentaryUnlockCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            if (ResultSet == "OK") {
                fnShowAlertInfo('Door unlocked successfully.');
                var deviceID = $("#AccessViewDeviceList").val();
                var dataItem = { "deviceId": deviceID };
                $.post(accessGetAccessDetailsUrl, dataItem, GetAccDetailsMomentaryCallback, "json")

                document.getElementById('my_overlay').style.visibility = 'hidden';
                document.getElementById('dvmomentaryUnlock').style.visibility = 'hidden';
            }
        }
    }
}

function GetAccDetailsMomentaryCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var grid = $("#grdAccess").data("kendoGrid");
            grid.dataSource.data(ResultSet);
        }
    }
}

function ShowAccessDescription() {
    $("#AccessDescriptionwindow").data("kendoWindow").open().center();
}

function fnAddAccessDefaultValuesClick() {
    var parameters;
    var parameters = { "DeviceId": $("#AccessViewDeviceList").data("kendoComboBox").value(), "InternalName": "ACCESSCONTROL", "ControlName": "AccessViewDeviceList" };
    $.post(accessSaveDefaultValueUrl, parameters, null, "json");
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}
function fnClearAccessDefaultValuesClick() {
    var parameters;
    var parameters = { "InternalName": "ACCESSCONTROL", "ControlName": "AccessViewDeviceList" };
    $.post(accessClearSaveDefaultValueUrl, parameters, ClearAccessDefaultValueCallBack, "json");
}

function SaveDefaultValueCallback(ResultSet) {
    // Empty function for call back of save defaults
}
function ClearAccessDefaultValueCallBack(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        fnShowAlertInfo(ResultSet);
    }
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}

function GetAccessDeviceTypes() {
    $.post(accessGetDevicesUrl, null, GetAccessDevicesByUserCallback, "json");
}

function GetAccessDevicesByUserCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboAccessDeviceList = $("#AccessViewDeviceList").data("kendoComboBox");
            Diebold.Access.setDevices(ResultSet);
            cboAccessDeviceList.dataSource.data(ResultSet);
            if (ResultSet[0] != null) {
                var selectedItem = ResultSet[0].DefaultSelectedValue;
                cboAccessDeviceList.value(selectedItem);
                var selectedAccessdefaultitemID = 0;
                selectAccessDevice(ResultSet);
            }
        }
    }
}
function openAcessDeviceList() {
    if ($('#mainBody .k-animation-container #AccessViewDeviceList-list').length > 0) {
        if ($('#mainBody > #AccessViewDeviceList-list').length > 0) {
            $('#AccessViewDeviceList-list').remove();
        }
    }
    $('#AccessViewDeviceList-list').prepend("<div id='DDAccessheader'><div id='rowcell'>Device</div><div id='rowcell'>Site</div><div id='rowcell'>Address 1</div><div id='rowcell'>Address 2</div><div id='rowcell'>City</div><div id='rowcell'>State</div><div id='rowcell' style='border-right:0;'>Zip</div></li>");
}
function closeAcessDeviceList() {
    $('#DDAccessheader').remove();
}

function AccessselectReport(e) {
    var dataItem = { "DeviceId": this.dataItem(e.item.index()).Id };
}

function GetDeviceDetailbyAccessDeviceIdCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var grid = $("#grdAccess").data("kendoGrid");
            grid.dataSource.data(ResultSet);
        }
    }
}
function GetPollingDetailbyAccessDeviceIdCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            $("#lblAccessPollingStatus").val('');
            $("#lblAccStatus").val('');

            $("#lblAccessPollingStatus").text(ResultSet.PollingStatus);
            $("#lblAccStatus").text(ResultSet.Status);
        }
    }
}

function selectAccessDevice(e) {
    
    var deviceID = $("#AccessViewDeviceList").val();
    if (e != null) {
        $("#lbldvType").val('');
        if (e.length == undefined) {            
            var selectedAccessInfo = this.select();
            var deviceType = this.dataItem(selectedAccessInfo).DeviceType;
            $("#lbldvType").val('');            
            $("#lbldvType").text(deviceType);
        }
        else if (e != null) {
            for (var i = 0; i < e.length; i++) {
                if (e[i].Id == deviceID) {
                    $("#lbldvType").text(e[i].DeviceType);                    
                }
            }
        }
    }
    else {
        if (e == null) {
            
        }
    }
    
    
   
    
    if (deviceID != '') {
        BlockElement("dvAccessViewContent", 'Connecting...');
        var dataItem = { "deviceId": deviceID };
        $.post(accessDoorUrl, dataItem, GetAccessDevicePollingStatus, "json")
            .done(function (response) { UnBlockElement("dvAccessViewContent"); })
            .fail(function (response) { UnBlockElement("dvAccessViewContent"); });
    }
}

function fnAccessRefreshClick() {
    var SelectedAccessDeviceId = $("#AccessViewDeviceList").data("kendoComboBox").value();
    if (SelectedAccessDeviceId != undefined && SelectedAccessDeviceId != null && SelectedAccessDeviceId != "") {
        BlockElement("dvAccessViewContent", 'Connecting...');
        var AccessDataItem = { "deviceId": SelectedAccessDeviceId };
        $.post(accessGetAccessDetailsUrl, AccessDataItem, GetAccessDevicePollingStatus, "json")
        .done(function (response) { UnBlockElement("dvAccessViewContent"); })
        .fail(function (response) { fnShowAlertInfo(response.Message); UnBlockElement("dvAccessViewContent"); });
    }
    else {
        fnShowAlertInfo('Please select a device before refreshing.');
        return false;
    }
    jQuery('#dvSettings').dialog('close');
}

function fnAccessPollingStatus(ResultSet) {
    document.getElementById('lblAccessPollingStatus').innerHTML = "";
    document.getElementById('lblAccStatus').innerHTML = "";
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            $("#lblAccessPollingStatus").text(ResultSet.PollingStatus);
            $("#lblAccStatus").text(ResultSet.Status);
        }
    }
}

function GetAccessDevicePollingStatus(ResultSet) {    
    var currentDevice = Diebold.Access.getSelectedDevice();

    // console.log(currentDevice);
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
    }
    $.post(accessPollingUrl, fnAccessPollingStatus, "json")
        .done(function (response) { UnBlockElement("dvAccessViewContent"); })
        .fail(function (response) { UnBlockElement("dvAccessViewContent"); });
    var grid = $("#grdAccess").data("kendoGrid");
    grid.dataSource.data(ResultSet);
    var selectedDevice = Diebold.Access.getSelectedDevice();
}

function fnGetAccessPortletReport() {
    var deviceID = $("#AccessViewDeviceList").val();
    var ReportID = $("#AccessReportType").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device.");
    }
    else if (ReportID == '') {
        fnShowAlertInfo("Please select report type.");
    }
    else {
        var accessStartDate = $("#AccessStartdatetimepicker").val();
        var accessEndDate = $("#AccessEnddatetimepicker").val();

        var accesscurrentdate_date = new Date();
        var accessCurrentmonth = accesscurrentdate_date.getMonth() + 1;
        var accesscurrentdate = accesscurrentdate_date.getDate();
        var fullYear = accesscurrentdate_date.getFullYear();
        var currentDt = accessCurrentmonth + "/" + accesscurrentdate + "/" + fullYear;

        var accessdiff = new Date(accessEndDate) - new Date(accessStartDate);
        var Accdtseconds = Math.round((accessdiff) / 1000);
        var accdtMin = Accdtseconds / 60;
        var dthrs = accdtMin / 60;

        if (new Date(accessEndDate) < new Date(accessStartDate)) {
            fnShowAlertInfo("Start date should not be greater than End date.");
        }
        else if (accessEndDate > currentDt || accessStartDate > currentDt) {
            fnShowAlertInfo('Start date and End date should not be greater than current date.');
        }
        else if (dthrs > 24 && (accessEndDate == currentDt)) {
            fnShowAlertInfo("Date difference between start and end date cannot be more than 24 hrs.");
        }
        else if ((accessEndDate != currentDt) && (accessStartDate != accessEndDate)) {
            fnShowAlertInfo("Date difference between start and end date cannot be more than 24 hrs.");
        }
        else {
            var dataItem = { "startDateTime": accessStartDate, "endDateTime": accessEndDate, "deviceId": deviceID };
            BlockElement("dvAccessViewContent", 'Connecting...');
            $.post(accessGetAccessControlReportUrl, dataItem, GetAccessReportbyCallback, "json");
        }
    }
}


function GetAccessReportbyCallback(ResultSet) {
    UnBlockElement("dvAccessViewContent");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            centerViewPortDialog("#dvAccRpt");
            $("#wndAccessreport").data("kendoWindow").center().open();
            var gridAccessdet = $("#grdAccdetReports").data("kendoGrid");
            gridAccessdet.dataSource.data(ResultSet);
        }
    }
}
function fnCloseAccessreport() {
    $("#wndAccessreport").data("kendoWindow").close();
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });

    $("#dvAccRpt").css({
        "display": "none",
        "visibility": "hidden"
    });

}
function onCloseAccess(e) {
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function fnAccessUserpinValidation() {
    if ($('#txtAccessUserpin').val() == '') {
        fnShowAlertInfo('User Pin is mandatory.');
        return false;
    }
    else if ($('#txtAccessUserpin').val().length < 4 || $('#txtAccessUserpin').val().length > 6) {
        $("#txtAccessUserpin").val('');
        fnShowAlertInfo("Please enter a valid user pin which accepts 4 to 6 digits");
        return false;
    }
    else {
        var existingValue = $("#hdnAccessSubmitClick").val();

        if (existingValue == '') {
            existingValue = 1;
        }
        else {
            existingValue = Number(existingValue) + 1;
        }
        $("#hdnAccessSubmitClick").val(existingValue);

        var submitClick = $("#hdnAccessSubmitClick").val();
    }
    var deviceId = $("#AccessViewDeviceList").val();

    var dataItem = { "UserPin": $('#txtAccessUserpin').val(), "submitVal": submitClick, "deviceId": deviceId };
    BlockElement("dvAccessUserPinWindowContent", 'Connecting...');
    $.post(accessValidateUserPinUrl, dataItem, GetAccessUserpinCallback, "json");
}

function GetAccessUserpinCallback(ResultSet) {
    UnBlockElement("dvAccessUserPinWindowContent");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            if (ResultSet == "logoff") {
                document.getElementById('dvAccessUserPinWindow').style.visibility = 'hidden';
                document.getElementById('my_overlay').style.visibility = 'hidden';
                location.href = accessLogoffUri;
            }
            else if (ResultSet == "false") {
                fnShowAlertInfo('Invalid User Pin');
            }
            else if (ResultSet == "true") {

                document.getElementById('dvAccessUserPinWindow').style.visibility = 'hidden';
                document.getElementById('my_overlay').style.visibility = 'hidden';
                fnShowMomentaryUnlock();
            }
        }
    }
}

function fnAccessOpenUserPin(door) {
    $("#hdnAccessSubmitClick").val('');
    $("#hdnAccessDoorName").val('');
    $("#txtAccessUserpin").val('');
    $("#hdnAccessDoorName").val(door.id);
    centerViewPortDialog("#dvAccessUserPinWindow");
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}
function hideAccessUserPinOverlay() {
    document.getElementById('dvAccessUserPinWindow').style.visibility = 'hidden';
    document.getElementById('my_overlay').style.visibility = 'hidden';
}
function uncheckAllByParent(aId, aChecked) {
    var collection = document.getElementById(aId).getElementsByTagName('INPUT');
    for (var x = 0; x < collection.length; x++) {
        if (collection[x].type.toUpperCase() == 'CHECKBOX')
            collection[x].checked = aChecked;
    }
}

function fnOpenAccessDetails() {
    $.post(accessGetAccessDevicesforSearchUrl, null, GetAccessSearchCallback, "json");
}
function GetAccessSearchCallback(ResultSet) {
    var grdSearchAccessDeviceDetails = $("#grdSearchAccessDeviceDetails").data("kendoGrid");
    grdSearchAccessDeviceDetails.dataSource.data(ResultSet);
    centerViewPortDialog("#dvSearchAccessDevices");
}
function grdSearchAccessChanged() {
    
    var selectedAccessInfo = this.select();
   
    var deviceId = this.dataItem(selectedAccessInfo).Id;
    var deviceName = this.dataItem(selectedAccessInfo).Name;
    var deviceType = this.dataItem(selectedAccessInfo).DeviceType;
    $("#lbldvType").text(deviceType);
    var cboSearchAccessList = $("#AccessViewDeviceList").data("kendoComboBox");
    cboSearchAccessList.value(deviceName);
    var selectedAccessInfo = deviceId;       
    selectAccessDevice(null);
    $("#dvSearchAccessDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function grdCHListChanged() {
    var selectedCardHolderInfo = this.select();
    CHid = this.dataItem(selectedCardHolderInfo).CardHolderId;
    CHfirstName = this.dataItem(selectedCardHolderInfo).firstName;
    CHlastName = this.dataItem(selectedCardHolderInfo).lastName;
    CHNumber = this.dataItem(selectedCardHolderInfo).cardNumber;
    CHListPin = this.dataItem(selectedCardHolderInfo).pin;
    CHListActivationDate = this.dataItem(selectedCardHolderInfo).cardActivationDate;
    CHListexpirationDate = this.dataItem(selectedCardHolderInfo).cardExpirationDate;
    CHListisActive = this.dataItem(selectedCardHolderInfo).isActive;
    CHListaccessGroupId = this.dataItem(selectedCardHolderInfo).accessGroupId;

    var accessDeviceID = $("#AccessViewDeviceList").val();
    document.getElementById('dvMdfyCrdHldrList').style.visibility = 'hidden';
    centerViewPortDialog("#dvMdfyCrdHldrMdFlds");
    BlockElement("dvCardholderModify2Content", 'Connecting...');
    $("#dvErrCHModifyflds ul").empty();
    $("#txtStep2CHID").val('');
    $("#txtStep2CHfirstName").val('');
    $("#txtStep2CHlastName").val('');
    $("#txtStep2CHcardNumber").val('');
    $("#txtStep2CHPin").val('');
    $("#txtStep2CHID").val(CHid);
    $("#txtStep2CHfirstName").val(CHfirstName);
    $("#txtStep2CHlastName").val(CHlastName);
    $("#txtStep2CHcardNumber").val(CHNumber);
    $("#txtStep2CHPin").val(CHListPin);
    $("#CHModifyActivationdatetimepicker").val(CHListActivationDate);
    $("#CHModifyExpirationdatetimepicker").val(CHListexpirationDate);
    $("#chkCHActiveModify").checked = CHListisActive;
    var parameters = { "deviceId": accessDeviceID };

    $.post(accessGetGroupListUrl, parameters, GetAccessGroupListCallback_Modify2, "json");
}
function grdDMPXRCHListChanged() {
    var selectedCardHolderInfo = this.select();
    CHid = this.dataItem(selectedCardHolderInfo).CardHolderId;   
    CHlastName = this.dataItem(selectedCardHolderInfo).lastName;
    CHNumber = this.dataItem(selectedCardHolderInfo).cardNumber;
    CHListaccessGroupId = this.dataItem(selectedCardHolderInfo).accessGroupId;

    var accessDeviceID = $("#AccessViewDeviceList").val();
    //document.getElementById('dvMdfyCrdHldrList').style.visibility = 'hidden';
    document.getElementById('dvDMPXRMdfyCrdHldrList').style.visibility = 'hidden';    
    centerViewPortDialog("#dvDMPXRMdfyCrdHldrMdFlds");
    BlockElement("dvDMPXRCardholderModify2Content", 'Connecting...');
    $("#dvErrCHModifyflds ul").empty();

    $("#txtStep2DMPXRCHID").val('');
    $("#txtStep2DMPXRCHlastName").val('');
    $("#txtStep2DMPXRCHcardNumber").val('');        
    $("#txtStep2DMPXRCHID").val(CHid);
    $("#txtStep2DMPXRCHlastName").val(CHlastName);
    $("#txtStep2DMPXRCHcardNumber").val(CHNumber);

    var parameters = { "deviceId": accessDeviceID };
    $.post(accessGetGroupListUrl, parameters, GetDMPXRAccessGroupListCallback_Modify2, "json");    
}
function grdDeleteCHListChanged() {
    var selectedCardHolderForDelete = this.select();
    var DeletedCHid = this.dataItem(selectedCardHolderForDelete).CardHolderId;
    var DeletedCHfirstName = this.dataItem(selectedCardHolderForDelete).firstName;
    var DeletedCHlastName = this.dataItem(selectedCardHolderForDelete).lastName;
    var DeletedCHNumber = this.dataItem(selectedCardHolderForDelete).cardNumber;

    centerViewPortDialog("#dvDeleteCrdHldrConfirmation");
    document.getElementById('dvDeleteCrdHldrList').style.visibility = 'hidden';
    $("#txtDeleteFNameStep2").val('');
    $("#txtDeleteLNameStep2").val('');
    $("#txtDeleteCardNumStep2").val('');
    $("#txtDeleteCardholderID2").val('');
    $("#txtDeleteFNameStep2").val(DeletedCHfirstName);
    $("#txtDeleteLNameStep2").val(DeletedCHlastName);
    $("#txtDeleteCardNumStep2").val(DeletedCHNumber);
    $("#txtDeleteCardholderID2").val(DeletedCHid);
}
function grdDMPXRDeleteCHListChanged() {    
    var selectedCardHolderForDelete = this.select();
    var DeletedCHid = this.dataItem(selectedCardHolderForDelete).CardHolderId;
    //var DeletedCHfirstName = this.dataItem(selectedCardHolderForDelete).firstName;
    var DeletedCHlastName = this.dataItem(selectedCardHolderForDelete).lastName;
    var DeletedCHNumber = this.dataItem(selectedCardHolderForDelete).cardNumber;
        
    centerViewPortDialog("#dvDMPXRDeleteCrdHldrConfirmation");
    document.getElementById('dvDMPXRDeleteCrdHldrList').style.visibility = 'hidden';
    $("#txtDMPXRDeleteLNameStep2").val('');
    $("#txtDMPXRDeleteCardNumStep2").val('');
    $("#txtDMPXRDeleteCardholderID2").val('');
    
    $("#txtDMPXRDeleteLNameStep2").val(DeletedCHlastName);
    $("#txtDMPXRDeleteCardNumStep2").val(DeletedCHNumber);
    $("#txtDMPXRDeleteCardholderID2").val(DeletedCHid);
}
function grdUCListChanged1() {    
    var selectedUserCodeInfo = this.select();
    AccessUserName = this.dataItem(selectedUserCodeInfo).UserName;
    AccessUserCode = this.dataItem(selectedUserCodeInfo).UserCode;
    AccessUserNumber = this.dataItem(selectedUserCodeInfo).UserNumber;
    AccessProfileNumber = this.dataItem(selectedUserCodeInfo).ProfileNumber;

    centerViewPortDialog("#dvStep2ModifyUserCode");
    document.getElementById('dvModifyUserCodeList').style.visibility = 'hidden';
    $("#txtStep2EditUserName").val('');
    $("#txtStep2EditUserCode").val('');
    $("#txtStep2EditUserNum").val('');
    $("#txtStep2EditUserName").val(AccessUserName);
    $("#txtStep2EditUserCode").val(AccessUserCode);
    $("#txtStep2EditUserNum").val(AccessUserNumber);

    var intrusionDeviceID = $("#IntrusionCategories").val();
    var parameters = { "deviceId": intrusionDeviceID };
    BlockElement("dvStep2ModifyUCContent", 'Connecting...');
    $.post(intrusionGetProfileNumberListUrl, parameters, GetProfileNumberCallback1_ModifyStep2, "json");
}
function grdUCDeleteListChanged() {
    var deletedUserCodeInfo = this.select();

    var DeletedUserName = this.dataItem(deletedUserCodeInfo).UserName;
    var DeletedUserCode = this.dataItem(deletedUserCodeInfo).UserCode;
    var DeletedUserNumber = this.dataItem(deletedUserCodeInfo).UserNumber;
    DeletedProfileNumber = this.dataItem(deletedUserCodeInfo).ProfileNumber;
    document.getElementById('dvDeleteUserCodeList').style.visibility = 'hidden';
    centerViewPortDialog("#dvStep2DeleteUserCode");
    $("#txtStep2DeleteUserName").val('');
    $("#txtStep2DeleteUserCode").val('');
    $("#txtStep2DeleteUserNum").val('');

    $("#txtStep2DeleteUserName").val(DeletedUserName);
    $("#txtStep2DeleteUserCode").val(DeletedUserCode);
    $("#txtStep2DeleteUserNum").val(DeletedUserNumber);

    var intruDeviceID = $("#IntrusionCategories").val();
    var parameters = { "deviceId": intruDeviceID };
    BlockElement("dvStep2DeleteUCContent", 'Connecting...');
    $.post(intrusionGetProfileNumberListUrl, parameters, GetProfileNumberCallback_DeleteStep2, "json");
}
function btnSearchAccessCancelClicked() {
    $("#dvSearchAccessDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}

//links
function fnCloseLinksInfo() {
    var mySplitResultset = {
        'input': 'Links'
    };
    $.post(linkClosePortletUrl, mySplitResultset, mySplitResultset, "json");

    $('#my-Links').hide();
    getWidthofColumn();
    return false;
}

function fnwndRemovelnkpopup() {
    $("#wndRemovelnk").data("kendoWindow").close();
}

function fntoggleLinks() {
    $('#divLinksContent').toggle();
}

function fnAddLinkItems() {
    $("#txtlnkName").val('');
    $("#txtlnkURL").val('');
    $("#Addlinkswindow").data("kendoWindow").center().open();
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}

// To open the popup for delete the links
function fnOpenLinkDeletePopup() {
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
    fnPopulateDeltePoupData();
}

function fnPopulateDeltePoupData() {
    $.post(linkPopulateLinksUrl, fnPopultePopupGrid, "json");
}

function fnPopultePopupGrid(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var grdlinks_Popup = $("#grdlinkDet_Popup").data("kendoGrid");
            grdlinks_Popup.dataSource.data(ResultSet);
            $("#wndRemovelnk").data("kendoWindow").center().open();
        }
    }
}

function fndeleteLinks() {

    //find the selected Links
    var grdGroupLevels = $("#grdlinkDet_Popup").data("kendoGrid");
    grdGroupLevels.tbody
                .find(":checked")
                .closest("tr")
                .each(function () {
                    var row = grdGroupLevels.dataItem(this);
                    var lnkSelectedID = row.Id;
                    ConfirmlnkOk(lnkSelectedID);
                });
}
function fnIsLinksSelected() {
    var linkFlag = false;
    var grdGroupLevels = $("#grdlinkDet_Popup").data("kendoGrid");
    grdGroupLevels.tbody
                .find(":checked")
                .closest("tr")
                .each(function () {
                    linkFlag = true;

                });
    return linkFlag;
}

function fnLinksPopupSave() {

    if (($('#txtlnkName').val() == '' || $.trim($('#txtlnkName').val()) == '') && $('#txtlnkURL').val() == '') {
        //To clear the text box when it contains only blank spaces
        $("#txtlnkName").val('');
        fnShowAlertInfo('Link Name is mandatory.' + '\n' + 'URL is mandatory.');
        return false;
    }
    else if ($('#txtlnkName').val() == '' || $.trim($('#txtlnkName').val()) == '') {
        //To clear the text box when it contains only blank spaces
        $("#txtlnkName").val('');
        fnShowAlertInfo('Link Name is mandatory.');
        return false;
    }
    else if ($('#txtlnkURL').val() == '') {
        fnShowAlertInfo('URL is mandatory.');
        return false;
    }


    if (validateInputFields()) {
        var dataItem = { "linkName": $('#txtlnkName').val(), "url": $('#txtlnkURL').val() };
        $.post(linkAddNewLinkUrl, dataItem, GetLinksDuplicatecheckCallback, "json");
    }
}
function GetLinksCallback(ResultSet) {
    var grdGroupLevels = $("#grdlnkDetails").data("kendoGrid");
    grdGroupLevels.dataSource.data(ResultSet);

    var grdlinks_Popup = $("#grdlinkDet_Popup").data("kendoGrid");
    grdlinks_Popup.dataSource.data(ResultSet);
}

function GetLinksDuplicatecheckCallback(ResultSet) {
    if (ResultSet == -1) {
        fnShowAlertInfo('Link has already been added with the same name.');
    }
    else if (ResultSet == -2) {
        fnShowAlertInfo('Link has already been added with the same URL.');
    }
    else {
        $("#Addlinkswindow").data("kendoWindow").close();
        $.post(linkGetActiveLinksByUserUrl, null, GetLinksCallback, "json");
    }
}

function btnlnkCancelClicked() {
    $("#Addlinkswindow").data("kendoWindow").close();
}
function validateInputFields() {
    var reURL = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/;

    //To restrict max link count for a user
    var grdLnkPopup = $("#grdlinkDet_Popup").data("kendoGrid");
    //To get total row count of a grid.
    var lnkPortletExist = grdLnkPopup.dataSource.total();

    if (lnkPortletExist >= lnkPortletMaxCount) {
        fnShowAlertInfo("Maximum Number of Allowed Links Exceeded.");
        return false;
    }
    if (!reURL.test($('#txtlnkURL').val())) {
        fnShowAlertInfo("Please enter valid url");
        return false;
    }
    return true;
}

function deleteLinksConfirmation() {

    if (fnIsLinksSelected()) {
        var wnd = $("#wndlinks").data("kendoWindow");
        wnd.content("<p style='height:100px'>Are you sure you want to delete the item(s)?</p><p>" +
                "<input type='button' value='Ok' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-right:4px;' onclick='fndeleteLinks()'>" +
                 "<input type='button' value='Cancel' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-left:5px;' onclick='ConfirmlnkCancel()'></p>");
        wnd.center().open();
    }
    else {
        fnShowAlertInfo('Please select a link to delete.');
    }
}
function ConfirmlnkCancel() {
    $("#wndlinks").data("kendoWindow").close();
}
function ConfirmlnkOk(linkId) {

    var data = { linkID: linkId };
    $.post(linkDeleteUrl, data, "json")
        .done(function (response) {

            //To reload the grid in main page
            var grid = $("#grdlnkDetails").data("kendoGrid");
            grid.dataSource.read();

            //To reload the links grid in popup
            var grid = $("#grdlinkDet_Popup").data("kendoGrid");
            grid.dataSource.read();

            // To close the popup windows
            $("#wndRemovelnk").data("kendoWindow").close();
            $("#wndlinks").data("kendoWindow").close();
        })
            .fail(function (response) {
                fnShowAlertInfo('Links not deleted.');
            });

}

function ShowlnkPortletDescription() {
    $("#LinksDescriptionwindow").data("kendoWindow").open().center();
}
function DisplayLinksURL(url) {

    // Script to test whether the URL starts with http://
    String.prototype.startsWith = function (prefix) {
        return this.indexOf(prefix) === 0;
    }

    var lnkurlCheck = url.startsWith("http");
    var newURL;
    if (!lnkurlCheck) {
        newURL = "http://" + url;
    }
    else {
        newURL = url;
    }
    window.open(newURL);
}

//RSS

function ShowRSSPortletDescription() {
    $("#wndRSSFeedDes").data("kendoWindow").open().center();
}
function fnwndRemoveRssFeedpopclose() {
    $("#wndRemoveRssFeed").data("kendoWindow").close();
}
function fnAddRssItems() {
    $("#txtrssFeedName").val('');
    $("#txtrssFeedURL").val('');

    $("#AddRSSFeedwindow").data("kendoWindow").center().open();

    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}

function deleteRSSFeedConfirmation() {

    if (fnIsRSSFeedSelected()) {
        var wnd = $("#wndRSSFeed").data("kendoWindow");
        wnd.content("<p style='height:100px'>Are you sure you want to delete the item?</p><p>" +
                "<input type='button' value='Ok' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-right:4px;' onclick='fndeleteRSSFeed()'>" +
                 "<input type='button' value='Cancel' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-left:5px;' onclick='ConfirmRSSFeedCancel()'></p>");
        wnd.center().open();
    }
    else {
        fnShowAlertInfo('Please select a RSS Feed to delete.');
    }
}
function fnIsRSSFeedSelected() {
    var rssFeedFlag = false;
    var grdGroupLevels = $("#grdRSSFeed_Popup").data("kendoGrid");
    grdGroupLevels.tbody
                .find(":checked")
                .closest("tr")
                .each(function () {
                    rssFeedFlag = true;

                });
    return rssFeedFlag;
}

function ConfirmRSSFeedCancel() {
    $("#wndRSSFeed").data("kendoWindow").close();
}

function fndeleteRSSFeed() {

    //find the selected Links
    var grdRSSFeedPopup = $("#grdRSSFeed_Popup").data("kendoGrid");
    grdRSSFeedPopup.tbody
                .find(":checked")
                .closest("tr")
                .each(function () {
                    var row = grdRSSFeedPopup.dataItem(this);
                    var lnkSelectedRSSID = row.Id;
                    ConfirmRSSOk(lnkSelectedRSSID);
                });
}

function ConfirmRSSOk(RssFeedId) {

    var data = { RssFeedId: RssFeedId };
    $.post(rssDeleteFeedUrl, data, "json")
        .done(function (response) {

            //To reload the grid in main page
            var grid = $("#grdRssFeed").data("kendoGrid");
            grid.dataSource.read();

            $.post(rssGetFeedCountUrl, null, GetRssCountPopupCallback, "json");

            // To close the popup windows
            $("#wndRemoveRssFeed").data("kendoWindow").close();
            $("#wndRSSFeed").data("kendoWindow").close();
        })
            .fail(function (response) {
                fnShowAlertInfo('RSS not deleted.');
            });

}

function fnCancelRSSItems() {
    $("#AddRSSFeedwindow").data("kendoWindow").close();
}
function fnRssFeedPopupSave() {
    if ($('#txtrssFeedName').val() == '' && $('#txtrssFeedURL').val() == '') {
        fnShowAlertInfo('RSS Feed Name is mandatory.' + '\n' + 'URL is mandatory.');
        return false;
    }
    else if ($('#txtrssFeedName').val() == '') {
        fnShowAlertInfo('RSS Feed Name is mandatory.');
        return false;
    }
    else if ($('#txtrssFeedURL').val() == '') {
        fnShowAlertInfo('URL is mandatory.');
        return false;
    }

    if (validateRSSInputFields()) {
        //To Re-bind RSS popup grid                
        $.post(rssGetFeedUrl, null, GetRssPopupCallback, "json");
        var dataItem = { "name": $('#txtrssFeedName').val(), "url": $('#txtrssFeedURL').val() };
        $.post(rssFeedCreateUrl, dataItem, GetRssCountPopupCallback, "json");
    }
}
function validateRSSInputFields() {

    var re = /^[0-9A-z\s]+$/;
    var reURL = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/;

    //To restrict max link count for a user
    var lnkExist = $("#RSSFeedCount").val();

    if (parseInt(lnkExist) >= parseInt(lnkMaxCount)) {
        fnShowAlertInfo("Maximum Number of Allowed RSS Feeds Exceeded.");
        return false;
    }
    //        if (!re.test($('#txtrssFeedName').val())) {
    //            fnShowAlertInfo("Please enter valid RssFeed");
    //            return false;
    //        }
    if (!reURL.test($('#txtrssFeedURL').val())) {
        fnShowAlertInfo("Please enter valid url");
        return false;
    }
    return true;
}
function GetRssCountPopupCallback(ResultSet) {
    if (ResultSet == -1) {
        fnShowAlertInfo('RSS Feed has already been added with the same name.');
    }
    else {
        var lnkExistcnt = ResultSet;
        $("#RSSFeedCount").val(lnkExistcnt);
        $("#AddRSSFeedwindow").data("kendoWindow").close();
        $.post(rssGetFeedDetailsUrl, null, GetRssFeedCallback, "json");
    }
}
function GetRssFeedCallback(ResultSet) {
    var grdGroupLevels = $("#grdRssFeed").data("kendoGrid");
    grdGroupLevels.dataSource.data(ResultSet);
}
function GetRssPopupCallback(ResultSet) {
    var grdRSSPopup = $("#grdRSSFeed_Popup").data("kendoGrid");
    grdRSSPopup.dataSource.data(ResultSet);
}
function fntoggleRSS() {
    $('#divRSSContent').toggle();
}
function fnCloseRSSFeed() {
    var mySplitResultset = {
        'input': 'RSS'
    };
    $.post(rssClosePortletUrl, mySplitResultset, mySplitResultset, "json");
    $('#my-RSSFeed').hide();
    getWidthofColumn();
    return false;
}

function DeleteRssFeed(RssFeedId) {
    var url = rssDeleteFeedUrl;
    var wnd = $("#wndRSSFeed").data("kendoWindow");
    wnd.content("<p style='height:100px'>Are you sure you want to delete this item(s)?</p><p>" +
                "<input type='button' value='Ok' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-right:4px;' onclick='javascript:ConfirmRssFeedOk(" + RssFeedId + ");'>" +
                 "<input type='button' value='Cancel' style='float:left;background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 15px 6px 22px;margin-left:5px;' onclick='ConfirmRssFeedCancel()'></p>");
    wnd.center().open();
}
function ConfirmRssFeedCancel() {
    $("#wndRSSFeed").data("kendoWindow").close();
}
function ConfirmRssFeedOk(RssFeedId) {
    var data = { RssFeedId: RssFeedId };
    $("#wndRSSFeed").data("kendoWindow").close();
    $.post(rssDeleteFeedUrl, data, "json")
        .done(function (response) {
            var grid = $("#grdRssFeed").data("kendoGrid");
            grid.dataSource.read();
        })
            .fail(function (response) {
                fnShowAlertInfo('Rss Feed not deleted.');
            });
}

function DisplayRssFeedURL(url) {

    // Script to test whether the URL starts with http://
    String.prototype.startsWith = function (prefix) {
        return this.indexOf(prefix) === 0;
    }

    var RssFeedurlCheck = url.startsWith("http");
    var newRssURL;
    if (!RssFeedurlCheck) {
        newRssURL = "http://" + url;
    }
    else {
        newRssURL = url;
    }
    window.open(newRssURL);
}

// To open the popup for delete the links
function fnOpenRSSDeletePopup() {
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
    fnGetRssLinks();
}

function fnGetRssLinks() {
    $.post(rssFeedPopupUrl, fnPopulateRssLinks, "json");
}

function fnPopulateRssLinks(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var grdRssLinks = $("#grdRSSFeed_Popup").data("kendoGrid");
            grdRssLinks.dataSource.data(ResultSet);
            $("#wndRemoveRssFeed").data("kendoWindow").center().open();
        }
    }
}

//Intrusion
function fnCloseIntrusiondet() {
    var mySplitResultset = {
        'input': 'Intrusion'
    };
    $.post(intrusionClosePortletUrl, mySplitResultset, mySplitResultset, "json");

    $('#IntrusionportletArea').hide();
    getWidthofColumn();
    return false;
}

function fntoggleIntrusiondet() {
    $('#Intrusion-content').toggle();
}

function GrdMasterSelectionChange() {
    $('#p_zone').show();
    $('#dvDetRow').show();
    var selectedArea = this.select();
    $("#hdnareaNumber").val(this.dataItem(selectedArea).AreaNumber);
    var dataItem = { "areNumber": this.dataItem(selectedArea).AreaNumber };
    BlockElement("Intrusion-content", 'Connecting...');
    $.post(intrusionGetZonesByAreaUrl, dataItem, GetZonesDetailbyMasterRoomIdCallback, "json");
}

function GetZonesDetailbyMasterRoomIdCallback(ResultSet) {
    UnBlockElement("Intrusion-content");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var grid = $("#grdDetailRow").data("kendoGrid");
            grid.dataSource.data(ResultSet);
        }
    }
}

function fnShowIntrusionpopup() {

    centerViewPortDialog("#dvIntrusionPopup");
    document.getElementById('dvIntrusionPopup').style.visibility = 'visible';
    document.getElementById('Intrusion_overlay').style.visibility = 'visible';
}



function ModifyUserCode() {
    var deviceID = $("#IntrusionCategories").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device");
    } else {
        $('#txtEditUserName').val('');
        $('#txtEditUserCode').val('');
        $("#dvErrUserCodeEdit ul").empty();
        var parameters = { "deviceId": deviceID, action: "modify" };
        BlockElement("dvAddUserCodeContent", 'Connecting...');
        $.ajax({
            url: "/intrusion/GetUserCodeView",
            data: parameters,
            dataType: "html",
            type: 'POST',
            success: function (data) {
                //UnBlockElement("#dvAddUserCodeContent");
                $("#dvStep1ModifyUserCode").html(data);
                centerViewPortDialog("#dvStep1ModifyUserCode");
            }
        });
        centerViewPortDialog("#dvStep1ModifyUserCode");
    }

}
function GetProfileNumberCallback1_ModifyStep2(ResultSet) {
    UnBlockElement("dvStep2ModifyUCContent");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {            
            var cboProfileNumListModify = $("#cboProfileNumberModify").data("kendoComboBox");
            cboProfileNumListModify.dataSource.data(ResultSet.result);
            cboProfileNumListModify.value(AccessProfileNumber);
        }
    }
}
function DeleteUserCode() {
    var deviceID = $("#IntrusionCategories").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device");
    } else {
        $('#txtDeleteUserName').val('');
        $('#txtDeleteUserCode').val('');
        $("#dvErrUserCodeDelete ul").empty();

        var parameters = { "deviceId": deviceID, action: "delete" };
        //BlockElement("dvAddUserCodeContent", 'Connecting...');
        $.ajax({
            url: "/intrusion/GetUserCodeView",
            data: parameters,
            dataType: "html",
            type: 'POST',
            success: function (data) {
                //UnBlockElement("#dvAddUserCodeContent");
                $("#dvStep1DeleteUserCode").html(data);
                centerViewPortDialog("#dvStep1DeleteUserCode");

            }
        });

        //centerViewPortDialog("#dvStep1DeleteUserCode");
    }
}
function GetProfileNumberCallback_DeleteStep2(ResultSet) {
    UnBlockElement("dvStep2DeleteUCContent");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboProfileNumListDelete = $("#cboProfileNumberDelete").data("kendoComboBox");
            cboProfileNumListDelete.dataSource.data(ResultSet);
            cboProfileNumListDelete.value(DeletedProfileNumber);
        }
    }
}
function hidePopup() {
    document.getElementById('my_overlay').style.visibility = 'hidden';
    document.getElementById('dvAddUserCode').style.visibility = 'hidden';
}

function hideStep1ModifyPopup() {
    document.getElementById('dvStep1ModifyUserCode').style.visibility = 'hidden';
    document.getElementById('my_overlay').style.visibility = 'hidden';
}
function hideIntrusionModifyOverlay() {
    document.getElementById('dvStep2ModifyUserCode').style.visibility = 'hidden';
    document.getElementById('my_overlay').style.visibility = 'hidden';
}

function hideDeleteOverlay() {
    document.getElementById('dvStep1DeleteUserCode').style.visibility = 'hidden';
    document.getElementById('my_overlay').style.visibility = 'hidden';
}

function hideIntrusionDeleteOverlay() {
    document.getElementById('dvStep2DeleteUserCode').style.visibility = 'hidden';
    document.getElementById('my_overlay').style.visibility = 'hidden';
}
function hideIntrusionUserPinOverlay() {
    document.getElementById('dvIntrusionUserPin').style.visibility = 'hidden';
    document.getElementById('my_overlay').style.visibility = 'hidden';
}
function GetIntrusionDeviceTypes() {
    $.post(intrusionGetDeviceListforIntrusionUrl, null, GetIntrusionDevicesByUserCallback, "json");
}

function GetIntrusionDevicesByUserCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboIntrusionDeviceList = $("#IntrusionCategories").data("kendoComboBox");
            Diebold.Intrusion.setDevices(ResultSet);
            cboIntrusionDeviceList.dataSource.data(ResultSet);
            if (ResultSet != null && ResultSet.length > 0 && ResultSet[0].DefaultSelectedValue != undefined
                && ResultSet[0].DefaultSelectedValue != null
                    && ResultSet[0].DefaultSelectedValue != "") {
                var selectedItem = ResultSet[0].DefaultSelectedValue;
                if (selectedItem != null && selectedItem != undefined) {
                    cboIntrusionDeviceList.value(selectedItem);
                    BlockElement("Intrusion-content", 'Connecting...');
                    intrusionDataItem = { "deviceId": selectedItem };
                    $.post(intrusionGetPlatformIntrusionDetailsUrl, intrusionDataItem, GetPollingDetailbyDeviceIdCallback, "json")
                        .done(function (response) { UnBlockElement("Intrusion-content"); })
                        .fail(function (response) { UnBlockElement("Intrusion-content"); });
                }
            }
        }
    }
}

function fnIntrusionRefreshClick() {
    var SelectedIntrusionDeviceId = $("#IntrusionCategories").data("kendoComboBox").value();
    if (SelectedIntrusionDeviceId != undefined && SelectedIntrusionDeviceId != null && SelectedIntrusionDeviceId != "") {
        BlockElement("Intrusion-content", 'Connecting...');
        var IntrusionDataItem = { "deviceId": SelectedIntrusionDeviceId };
        $.post(intrusionGetIntrusionDetailsUrl, IntrusionDataItem, GetPollingDetailbyDeviceIdCallback, "json")
        .done(function (response) { UnBlockElement("Intrusion-content"); })
        .fail(function (response) { fnShowAlertInfo(response.Message); UnBlockElement("Intrusion-content"); });
    }
    else {
        fnShowAlertInfo('Please select a device before refreshing.');
        return false;
    }
    jQuery('#dvSettings').dialog('close');
}

function fnAddIntrusionDefaultValuesClick() {
    var parameters = { "DeviceId": $("#IntrusionCategories").data("kendoComboBox").value(), "InternalName": "INTRUSION", "ControlName": "IntrusionCategories" };
    $.post(intrusionSaveIntrusionDefaultValueUrl, parameters, null, "json");

    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}
function fnClearIntrusionDefaultValuesClick() {
    var parameters = { "InternalName": "INTRUSION", "ControlName": "IntrusionCategories" };
    $.post(intrusionClearIntrusionDefaultValueUrl, parameters, ClearIntrusionDefaultValueCallBack, "json");
}
function ClearIntrusionDefaultValueCallBack(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        fnShowAlertInfo(ResultSet);
    }
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}
function fnUpinWindow(areanum, arm) {

    $("#hdnareaNumber").val('');
    $("#hdnareaArm").val('');

    $("#hdnareaNumber").val(areanum);
    $("#hdnareaArm").val(arm);
    $("#hdnsubmitClick").val('');

    $("#txtUserpin").val('');
    centerViewPortDialog("#dvIntrusionUserPin");
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}

function fnByPassReset(ZoneNumber, byPass) {

    $("#hdnzoneNumber").val('');
    $("#hdnByPass").val('');

    $("#hdnzoneNumber").val(ZoneNumber);
    $("#hdnByPass").val(byPass);

    var deviceId = $("#IntrusionCategories").val();
    var zoneNum = ZoneNumber;
    var byPass = $("#hdnByPass").val();

    var dataItem = { "zoneNumber": zoneNum, "deviceId": deviceId, "byPass": byPass };
    BlockElement("Intrusion-content", 'Connecting...');
    $.post(intrusionZoneByPassUrl, dataItem, GetzoneByPassCallback, "json");
}

function GetzoneByPassCallback(ResultSet) {
    UnBlockElement("Intrusion-content");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            if (ResultSet == "OK") {
                var dataItem = { "deviceId": $("#IntrusionCategories").val() };
                BlockElement("Intrusion-content", 'Connecting...');
                $.post(intrusionGetIntrusionDetailsUrl, dataItem, GetIntruDetailsbyZoneByPassCallback, "json");
            }
        }
    }
}
function GetIntruDetailsbyZoneByPassCallback(ResultSet) {
    UnBlockElement("Intrusion-content");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var AreaNumber = $("#hdnareaNumber").val();
            var dataItem = { "areNumber": AreaNumber };
            BlockElement("Intrusion-content", 'Connecting...');
            $.post(intrusionGetZonesByAreaUrl, dataItem, GetZonesDetailbyMasterRoomIdCallback, "json");
        }
    }
}

function fnAddDefaultValues() {
    var parameters = { "DeviceId": $("#IntrusionCategories").data("kendoComboBox").value(), "InternalName": "INTRUSION", "ControlName": "IntrusionCategories" };
    $.post(intrusionAddDefaultValuesUrl, parameters, null, "json");

    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}

var intrusionDataItem;
function selectIntrusionDevice(e) {
    BlockElement("Intrusion-content", 'Connecting...');
    intrusionDataItem = { "deviceId": $("#IntrusionCategories").val() };
    $.post(intrusionGetPlatformIntrusionDetailsUrl, intrusionDataItem, GetPollingDetailbyDeviceIdCallback, "json")
        .done(function (response) { UnBlockElement("Intrusion-content"); })
        .fail(function (response) { UnBlockElement("Intrusion-content"); });
}
function LoadIntrusionDeviceDetails(deviceID) {
    BlockElement("Intrusion-content", 'Connecting...');
    var intruDataItem = { "deviceId": deviceID };
    $.post(intrusionGetPlatformIntrusionDetailsUrl, intruDataItem, GetPollingDetailbyDeviceIdCallback, "json")
        .done(function (response) { UnBlockElement("Intrusion-content"); })
        .fail(function (response) { UnBlockElement("Intrusion-content"); });
}
function fnPollingStatus(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            $("#lblIntruPollingStatus").text(ResultSet.PollingStatus);
            $("#lblIntruStatus").text(ResultSet.Status);
        }
    }
}

function GetPollingDetailbyDeviceIdCallback(ResultSet) {
    document.getElementById('lblIntruStatus').innerHTML = "";
    document.getElementById('lblIntruPollingStatus').innerHTML = "";
    $("#grdMasterRoom").data("kendoGrid").dataSource.data([]);
    $("#grdDetailRow").data("kendoGrid").dataSource.data([]);

    intrusionDataItem = { "deviceId": $("#IntrusionCategories").val() };
    var currentDevice = Diebold.Intrusion.getSelectedDevice();

    // console.log(currentDevice);
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            $.post(intrusionPollingUrl, fnPollingStatus, "json")
                .done(function (response) { UnBlockElement("Intrusion-content"); })
                .fail(function (response) { UnBlockElement("Intrusion-content"); });

            var grid = $("#grdMasterRoom").data("kendoGrid");
            grid.dataSource.data(ResultSet);

            var selectedDevice = Diebold.Intrusion.getSelectedDevice();
            var columnsGrid = $("#grdDetailRow").data("kendoGrid");
            if (selectedDevice.Device == "videofied01") {
                // 0 = Name, 1 = Bypass, 2 = Satus, 3 = Commands
                // Hide Bypass
                columnsGrid.hideColumn(1);
                // Show Status and Command
                columnsGrid.showColumn(2);
                columnsGrid.showColumn(3);
                $(".intru-buttons input, .cbArmed").prop("disabled", true);

            } else {
                // Show Bypass
                columnsGrid.showColumn(1);
                // Hide Status and Command
                columnsGrid.hideColumn(2);
                columnsGrid.hideColumn(3);
                $(".intru-buttons input, .cbArmed").prop("disabled", false);
            }
        }
    }
    UnBlockElement("Intrusion-content");
}

function ShowIntrusionDescription() {
    $("#IntrusionDescriptionwindow").data("kendoWindow").open().center();
}
function openDDIntrusionheader() {
    if ($('#mainBody .k-animation-container #IntrusionCategories-list').length > 0) {
        if ($('#mainBody > #IntrusionCategories-list').length > 0) {
            $('#IntrusionCategories-list').remove();
        }
    }
    $('#IntrusionCategories-list').prepend("<div id='DDIntrusionheader'><div id='rowcell'>Device</div><div id='rowcell'>Site</div><div id='rowcell'>Address1</div><div id='rowcell'>Address2</div><div id='rowcell'>City</div><div id='rowcell'>State</div><div id='rowcell' style='border-right:0;'>Zip</div></li>");
}
function closeDDIntrusionheader() {
    $('#DDIntrusionheader').remove();
}

function fnUserpinValidation() {
    if ($('#txtUserpin').val() == '') {
        $("#txtUserpin").val('');
        fnShowAlertInfo("User Pin is mandatory.");
        return false;
    }
    else if ($('#txtUserpin').val().length < 4 || $('#txtUserpin').val().length > 6) {
        $("#txtUserpin").val('');
        fnShowAlertInfo("Please enter a valid user pin which accepts 4 to 6 digits");
        return false;
    }
    else {
        var existingValue = $("#hdnsubmitClick").val();

        if (existingValue == '') {
            existingValue = 1;
        }
        else {
            existingValue = Number(existingValue) + 1;
        }
        $("#hdnsubmitClick").val(existingValue);

        var submitClick = $("#hdnsubmitClick").val();
    }
    var areanum = $("#hdnareaNumber").val();
    var areaArm = $("#hdnareaArm").val();
    var deviceId = $("#IntrusionCategories").val();

    var dataItem = { "UserPin": $('#txtUserpin').val(), "submitVal": submitClick, "areaNumber": areanum, "arm": areaArm, "deviceId": deviceId };
    BlockElement("IntrusionUserPinContent", 'Connecting...');
    $.post(intrusionValidateUserPinUrl, dataItem, GetUserpinCallback, "json");
}
function GetUserpinCallback(ResultSet) {
    UnBlockElement("IntrusionUserPinContent");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            if (ResultSet == "logoff") {
                document.getElementById('dvIntrusionUserPin').style.visibility = 'hidden';
                document.getElementById('my_overlay').style.visibility = 'hidden';
                location.href = intrusionEditUri;
            }
            else if (ResultSet == "false") {
                fnShowAlertInfo("Invalid User Pin.");
            }
            else if (ResultSet == "OK") {
                document.getElementById('dvIntrusionUserPin').style.visibility = 'hidden';
                document.getElementById('my_overlay').style.visibility = 'hidden';
                intrusionDataItem = { "deviceId": $("#IntrusionCategories").val() };
                BlockElement("Intrusion-content", 'Connecting...');
                $.post(intrusionGetIntrusionDetailsUrl, intrusionDataItem, GetIntruDetailsbyDeviceIdCallback, "json");
            }
        }
    }
}

function GetIntruDetailsbyDeviceIdCallback(ResultSet) {
    UnBlockElement("Intrusion-content");
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var grid = $("#grdMasterRoom").data("kendoGrid");
            grid.dataSource.data(ResultSet);
        }
    }
}

function fnGetIntrusionReport() {

    var startDate = $("#Startdatetimepicker").val();
    var endDate = $("#Enddatetimepicker").val();
    var deviceID = $("#IntrusionCategories").val();
    var ReportTypeId = $("#ReportType").val();
    if (deviceID == '') {
        fnShowAlertInfo("Please select a device")
    }
    else if (ReportTypeId == '') {
        fnShowAlertInfo("Please select report type.")
    }
    else {

        var intrusioncurrentdate_date = new Date();
        var intrusionCurrentmonth = intrusioncurrentdate_date.getMonth() + 1;
        var intrusioncurrentdate = intrusioncurrentdate_date.getDate();
        var intrusionfullYear = intrusioncurrentdate_date.getFullYear();
        var intrusioncurrentDt = intrusionCurrentmonth + "/" + intrusioncurrentdate + "/" + intrusionfullYear;

        var intrusiondiff = new Date(endDate) - new Date(startDate);
        var intrusiondtseconds = Math.round((intrusiondiff) / 1000);
        var intrusiondtMin = intrusiondtseconds / 60;
        var intrsuiondthrs = intrusiondtMin / 60;

        if (new Date(endDate) < new Date(startDate)) {
            fnShowAlertInfo("Start date should not be greater than End date.");
        }
        else if (endDate > intrusioncurrentDt || startDate > intrusioncurrentDt) {
            fnShowAlertInfo('Start date and End date should not be greater than current date.');
        }
        else if (intrsuiondthrs > 24 && (endDate == intrusioncurrentDt)) {
            fnShowAlertInfo("Date difference between start and end date cannot be more than 24 hrs.");
        }
        else if ((endDate != intrusioncurrentDt) && (startDate != endDate)) {
            fnShowAlertInfo("Date difference between start and end date cannot be more than 24 hrs.");
        }
        else {
            var dataItem = { "startDateTime": startDate, "endDateTime": endDate, "deviceId": deviceID };
            BlockElement("Intrusion-content", 'Connecting...');
            $.post(intrusionGetIntrusionReportUrl, dataItem, GetIntrusionReportbyCallback, "json");
        }
    }
}

function GetIntrusionReportbyCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            centerViewPortDialog("#dvPnlReport");

            var gridIntrusion = $("#grdIntruReports").data("kendoGrid");
            gridIntrusion.dataSource.data(ResultSet);
        }
    }
    UnBlockElement("Intrusion-content");
}
function fnCloseIntrusionreport() {
    $("#dvPnlReport").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function onClose(e) {
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function fnOpenIntrusionDetails() {
    $.post(intrusionGetIntrusionDeviceListforSearchUrl, null, GetIntrusionDeviceforSearchCallback, "json");
}
function GetIntrusionDeviceforSearchCallback(ResultSet) {
    var grdSearchIntrusionDeviceDetails = $("#grdSearchIntrusionDeviceDetails").data("kendoGrid");
    Diebold.Intrusion.setDevices(ResultSet);
    grdSearchIntrusionDeviceDetails.dataSource.data(ResultSet);
    centerViewPortDialog("#dvSearchIntrusionDevices");
}
function grdSearchIntrusionChanged() {
    var selectedIntruDeviceInfo = this.select();
    var deviceId = this.dataItem(selectedIntruDeviceInfo).Id;
    var deviceName = this.dataItem(selectedIntruDeviceInfo).Name;
    var cboSearchIntrusionList = $("#IntrusionCategories").data("kendoComboBox");
    cboSearchIntrusionList.value(deviceName);
    var selectedIntruDeviceInfo = deviceId;
    LoadIntrusionDeviceDetails(selectedIntruDeviceInfo);
    $("#dvSearchIntrusionDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function btnSearchIntrusionCancelClicked() {
    $("#dvSearchIntrusionDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}

// Alert
var deviceSelectedData;
function fnCloseAlerts() {
    var mySplitResultset = {
        'input': 'Alerts'
    };
    $.post(alertClosePortletUrl, mySplitResultset, mySplitResultset, "json");

    $('#AlertsInfo').hide();
    getWidthofColumn();
    return false;
}

function fntoggleAlerts() {
    $('#divAlertContent').toggle();
}

function AlertGridName_change(e) {
    var grid = e.sender;
    var currentDataItem = grid.dataItem(this.select());
}

function AssignCamerastoGrid(ResultSet) {
    var grid = $("#grdAlerts").data("kendoGrid");
    grid.dataSource.data(ResultSet);
    UnBlockElement("AlertsportletArea");
}

function DeviceTypeChanged(e) {
    BlockElement("AlertsportletArea", 'Connecting...');
    var dataItem = { "deviceId": this.dataItem(e.item.index()).Id };
    deviceSelectedData = { "deviceId": dataItem.deviceId };
    $.post(alertGetActiveAlertsbyDeviceIdUrl, deviceSelectedData, "application/html; charset=utf-8")
            .done(function (response) { AssignCamerastoGrid(response) })
            .fail(function (response) { alert(response); });

}

function GridAlertTypeChanged(e) {
    var grid = e.sender;
    var currentDataItem = grid.dataItem(this.select());
    var data = { "id": currentDataItem.Id };
    $.post(alertGetAllDeviceSiteDetailsbyParentTypeUrl, data, "application/html; charset=utf-8")
            .done(function (response) { bindSiteInformation(response) })
            .fail(function (response) { alert(response); });
}

function bindSiteInformation(ResultSet) {
    $("#lblLocation").text(ResultSet.Location);
    $("#lblAddress").text(ResultSet.LocationAddress);
    $("#lblLocationContact").text(ResultSet.ContactPersonName);
    $("#lblLocationContactemail").text(ResultSet.ContactPersonEmail);
    $("#lblLocationContactphone").text(ResultSet.ContactPersonPhone);
}

function fnAlertsRowMouseDown(chk, AlertId) {
    document.getElementById('htnSelectedRow').value = AlertId;
    if (chk.checked != 'True') {
        var currentRow = document.getElementById('htnSelectedRow').value;
        var combobox = $("#cmbDeviceTypeList").data("kendoComboBox");
        var dataItem = { "currentRow": currentRow, "deviceType": combobox.text(), "deviceId": combobox.value() };

        $.post(alertUpdateAlertStatusUrl, dataItem, "application/html; charset=utf-8")
                        .done(function (response) { fnRebindGrid(response) })
                        .fail(function (response) { alert(response); });
    }
    else {
        fnShowAlertInfo('Selected Item is already Acknowledged.');
        chk.value = 'True';
    }
}


function btnOkClicked() {
    var pinValue = document.getElementById('txtPin').value;
    var currentRow = document.getElementById('htnSelectedRow').value;
    var combobox = $("#cmbDeviceTypeList").data("kendoComboBox");
    var dataItem = { "pin": pinValue.toString(), "currentRow": currentRow, "deviceType": combobox.text(), "deviceId": combobox.value() };
    if (pinValue != "") {
        $.post(alertValidatePinNumberUrl, dataItem, "application/html; charset=utf-8")
                                .done(function (response) { fnRebindGrid(response) })
                .fail(function (response) { alert(response); });
    }
    else {
        alert('Please enter the Pin number');
    }
    // Close the popup immediatly and clear the pin textbox
    document.getElementById('txtPin').value = '';
    $("#Alertwindow").data("kendoWindow").close();
}

function InsertAlertHistory(result) {
    if (result == "Invalid Pin") {
        alert('Invalid User Pin Entered');
    }
    else (result == "InsertAlertHistory")
    {
        var grid = $("#grdAlerts").data("kendoGrid");
        grid.dataSource.data(result);
        //$("#Alertwindow").data("kendoWindow").close();
        // Need to enter the values in the alert history table -- (ResolvedAlert table)
        //                  var CurrentAlertId = document.getElementById('htnSelectedRow').value;
        //                  var combobox = $("#cmbDeviceTypeList").data("kendoComboBox");
        //                  var dataItem = { "AlertId": CurrentAlertId, "deviceType": combobox.text() };
        //                  var url = 'Url.Action("CreateAlertHistory", "Alerts")';
        //                   $.post(url, dataItem, "application/html; charset=utf-8")
        //                    .done(function (response) { fnRebindGrid(response) })
        //                    .fail(function (response) { alert(response); });
    }
}

function fnRebindGrid(result) {
    if (result == "Invalid Pin") {
        alert('Invalid User Pin Entered');
    }
    else {
        var grid = $("#grdAlerts").data("kendoGrid");
        grid.dataSource.data(result);
        // $("#Alertwindow").data("kendoWindow").close();
    }
}

function btnCancelClicked() {
    $("#Alertwindow").data("kendoWindow").close();
}

function GetAcknoledgedAlerts() {
    var selectedrow = $("#grdAlerts").find("tbody tr.k-state-selected");
    var goods = $('#grdAlerts').data("kendoGrid").dataItem(selectedrow);
    if (goods == undefined || goods == null || goods == 0) {
        alert('Please select atleast one alert.');
    }
    else {
        var goodsjson = goods.toJSON();
        var DeviceId = goodsjson.DeviceId;
        alertGetPreviouslyAcknoledgedAlertsUrl += "?DeviceId=" + DeviceId;
        $("#dvPreviouslyAckoledgedAlerts").load(alertGetPreviouslyAcknoledgedAlertsUrl)
                     .dialog({
                         dialogClass: 'PreviouslyAcknoledged-dialog',
                         autoOpen: true,
                         resizable: false,
                         title: 'Previously Acknowledged Alerts',
                         width: 500,
                         minheight: 250,
                         modal: true,
                         buttons: {
                             Close: function () {
                                 $(this).dialog("close");
                             }
                         }
                     });
    }
    var ClearURL = alertGetPreviouslyAcknoledgedAlertsUrl.split('?');
    alertGetPreviouslyAcknoledgedAlertsUrl = ClearURL[0];
}

function ShowAlertDescription() {
    $("#AlertDescriptionwindow").data("kendoWindow").open().center();
}

var _ParentTypeSelectedfromContextmenu = '';
function fnParentTypeSelected(pType) {
    _ParentTypeSelectedfromContextmenu = pType;
    BlockElement("divAlertContent", 'Connecting...');
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
    var searchAlert = document.getElementById('imgsearchAlertLogo');
    document.getElementById("chkAll").checked = false;
    var comboBox = $("#cmbDeviceTypeList").data("kendoComboBox");
    comboBox.enable(true);
    searchAlert.style.cursor = 'pointer';
    searchAlert.onclick = function () { fnOpenAlertDetails() };
    $("#grdAlerts").data("kendoGrid").dataSource.data([]);
    $('#cmbDeviceTypeList').data('kendoComboBox').value("Device List");
    $.ajax({
        url: alertGetAllDevicebyParentTypeUrl,
        type: 'POST',
        data: { parentType: _ParentTypeSelectedfromContextmenu },
        success: function (data) {
            $("#cmbDeviceTypeList").data("kendoComboBox").dataSource.data(data);
            UnBlockElement("divAlertContent");
        },
        error: function (errordata) {
            alert('Failure');
            UnBlockElement("divAlertContent");
        }
    });

}
function openAlertsDeviceList() {
    if ($('#mainBody .k-animation-container #cmbDeviceTypeList-list').length > 0) {
        if ($('#mainBody > #cmbDeviceTypeList-list').length > 0) {
            $('#cmbDeviceTypeList-list').remove();
        }
    }
    $('#cmbDeviceTypeList-list').prepend("<div id='DDAlertsheader'><div id='rowcell'>Device</div><div id='rowcell'>Site</div><div id='rowcell'>Address 1</div><div id='rowcell'>Address 2</div><div id='rowcell'>City</div><div id='rowcell'>State</div><div id='rowcell' style='border-right:0;'>Zip</div></li>");
}
function closeAlertsDeviceList() {
    $('#DDAlertsheader').remove();
}

function fnGetAllAlerts() {
    var searchAlert = document.getElementById('imgsearchAlertLogo');
    if (document.getElementById('chkAll').checked == true) {
        // fnParentTypeSelected('ALL');
        // disable cmbDeviceTypeList
        var comboBox = $("#cmbDeviceTypeList").data("kendoComboBox");
        comboBox.enable(false);
        searchAlert.onclick = null;
        searchAlert.style.cursor = 'default';
        $('#cmbDeviceTypeList').data('kendoComboBox').value("Device List");
        BlockElement("AlertsportletArea", 'Connecting...');
        var parentTypeName = parentType();
        $.post(alertGetALLActiveAlertsUrl, parentTypeName, "application/html; charset=utf-8")
            .done(function (response) { AssignCamerastoGrid(response) })
            .fail(function (response) { alert(response); UnBlockElement("AlertsportletArea"); });
    }
    else {
        var comboBox = $("#cmbDeviceTypeList").data("kendoComboBox");
        comboBox.enable(true);
        searchAlert.style.cursor = 'pointer';
        searchAlert.onclick = function () { fnOpenAlertDetails() };
        $("#grdAlerts").data("kendoGrid").dataSource.data([]);
        $('#cmbDeviceTypeList').data('kendoComboBox').value("Device List");
        UnBlockElement("AlertsportletArea");
    }
}
function fnOpenAlertDetails() {
    if (_ParentTypeSelectedfromContextmenu == '' || _ParentTypeSelectedfromContextmenu == undefined || _ParentTypeSelectedfromContextmenu == null) {
        _ParentTypeSelectedfromContextmenu = "DVR";
    }
    var dataItem = { "parentType": _ParentTypeSelectedfromContextmenu };
    BlockElement("divAlertContent", 'Connecting...');
    $.post(alertGetAllDevicebyParentTypeUrl, dataItem, GetAlertSearchCallback, "json");
}
function GetAlertSearchCallback(ResultSet) {
    UnBlockElement("divAlertContent");
    var grdSearchAlertDeviceDetails = $("#grdSearchAlertDeviceDetails").data("kendoGrid");
    grdSearchAlertDeviceDetails.dataSource.data(ResultSet);
    centerViewPortDialog("#dvSearchAlertDevices");
}
function grdSearchAlertChanged() {
    var selectedAlertInfo = this.select();
    var deviceId = this.dataItem(selectedAlertInfo).Id;
    var deviceName = this.dataItem(selectedAlertInfo).Name;
    var cboSearchAlertList = $("#cmbDeviceTypeList").data("kendoComboBox");
    cboSearchAlertList.value(deviceName);
    var selectedAlertInfo = deviceId;
    LoadActiveAlerts(selectedAlertInfo);
    $("#dvSearchAlertDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function btnSearchAlertCancelClicked() {
    $("#dvSearchAlertDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function LoadActiveAlerts(deviceID) {
    BlockElement("AlertsportletArea", 'Connecting...');
    var dataItem = { "deviceId": deviceID };
    $.post(alertGetActiveAlertsbyDeviceIdUrl, dataItem, "application/html; charset=utf-8")
            .done(function (response) { AssignCamerastoGrid(response) })
            .fail(function (response) { UnBlockElement("AlertsportletArea"); alert(response); });

}

//account detail
function fnGetAccountInfo() {
    BlockElement("divAccountDetContent", 'Connecting...');
    $.post(accountGetAccountDetailsUrl, fnPopulateAccountInfo, "json");
}

function fnPopulateAccountInfo(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            $("#lblCompanyName").text(ResultSet.CompanyName);
            $("#lblSiteCount").text(ResultSet.SiteCount);
            $("#lblIntrusionCount").text(ResultSet.IntrusionDevices);
            $("#lblHealthCount").text(ResultSet.HealthDevices);
            $("#lblAccessCount").text(ResultSet.AccessDevices);
            $("#imgCompanyLogo").attr("src", ResultSet.CompanyLogo);
        }
    }
    UnBlockElement("divAccountDetContent");
}

function fnAccountsMoreOptionsClick() {
    location.href = accountViewUserProfileUri;
}
function fntoggleAccountDet() {
    $('#divAccountDetContent').toggle();
}
function ShowAccountPortletDescription() {
    $("#AccountDescriptionwindow").data("kendoWindow").open().center();
}

// system summary

function GetSystemSummaryListCallback(ResultSet) {
    var dataItem;
    var cboSystemSummary = $("#DDLSystemSummary").data("kendoComboBox");
    cboSystemSummary.dataSource.data(ResultSet);
    if (ResultSet != null && ResultSet.length > 0 && ResultSet[0].DefaultSelectedValue != undefined
                && ResultSet[0].DefaultSelectedValue != null
                    && ResultSet[0].DefaultSelectedValue != "") {
        dataItem = { "DeviceType": ResultSet[0].DefaultSelectedValue };
    }
    else {
        dataItem = { "DeviceType": 'Access' };
    }
    cboSystemSummary.value(dataItem.DeviceType);
    BlockElement("divSysSummaryContent", 'Connecting...');
    $.post(systemSummaryGetSystemSummarybyDeviceTypeurl, dataItem, GetDeviceDetailbyDeviceIdCallback, "json");
}

function getAllParentDeviceType() {
    $.post(systemSummaryGetAccountListUrl, null, GetAccountListCallback, "json");
}

function DeviceSelected(e) {
    BlockElement("divSysSummaryContent", 'Connecting...');
    var dataItem = { "DeviceType": this.dataItem(e.item.index()).Name };
    $.post(systemSummaryGetSystemSummarybyDeviceTypeurl, dataItem, GetDeviceDetailbyDeviceIdCallback, "json");
}

function GetDeviceDetailbyDeviceIdCallback(ResultSet) {
    document.getElementById('divNoDevice').style.display = 'none';
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            //fnShowAlertInfo(ResultSet.Message);
            document.getElementById('dvsystemSymmaryChart').style.display = 'none';
            document.getElementById('divNoDevice').style.display = 'block';
            document.getElementById('divNoDevice').innerHTML = '<b>' + ResultSet.Message + '</b>';
            UnBlockElement("divSysSummaryContent");
            return;
        }
        if (ResultSet != 'Empty Report') {
            document.getElementById('dvNoReports').style.display = 'none';
            document.getElementById('dvsystemSymmaryChart').style.display = 'block';
            var chart = $("#SystemSummarychart").data("kendoChart");
            chart.dataSource.data(ResultSet);
        }
        else {
            document.getElementById('dvsystemSymmaryChart').style.display = 'none';
            $("#imgRecordsnotavailable").attr("src", systemSummaryImgNotAvalablehref);
            document.getElementById('dvNoReports').style.display = 'block';
        }
    }
    UnBlockElement("divSysSummaryContent");
}
function fnCloseSysSummary() {
    var mySplitResultset = {
        'input': 'SystemSummary'
    };
    $.post(systemSummaryClosePortletUrl, mySplitResultset, mySplitResultset, "json");

    $('#my-SysSummary').hide();
    getWidthofColumn();
    return false;
}

function fntoggleSysSummary() {
    $('#divSysSummaryContent').toggle();
}

function loadExtraParams() {
    return { DeviceType: 'Access' };
}
function ShowSummaryDesc() {
    $("#SiteSummDescription").data("kendoWindow").center().open();
}

// video health check portlet

function fnOpenVHCDetails() {
    $.post(videoHCGetDevicesByUserforSearchUrl, null, GetVHCSearchCallback, "json");
}
function GetVHCSearchCallback(ResultSet) {
    var grdSearchVHCDeviceDetails = $("#grdSearchVHCDeviceDetails").data("kendoGrid");
    grdSearchVHCDeviceDetails.dataSource.data(ResultSet);
    centerViewPortDialog("#dvSearchVHCDevices");
}
function grdSearchVHCChanged() {
    var selectedVHCInfo = this.select();
    var deviceId = this.dataItem(selectedVHCInfo).Id;
    var deviceName = this.dataItem(selectedVHCInfo).Name;
    var cboSearchVHCList = $("#cmbDevicelist").data("kendoComboBox");
    cboSearchVHCList.value(deviceName);
    var selectedVHCInfo = deviceId;
    LoadVHCDetails(selectedVHCInfo);
    $("#dvSearchVHCDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function btnSearchVHCCancelClicked() {
    $("#dvSearchVHCDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function LoadVHCDetails(deviceID) {
    BlockElement("divVideoHealthCheckContent", 'Connecting...');
    var VHCDataItem = { "deviceId": deviceID };
    $.post(videoHCGetPlatformDevicePollingStatusUrl, VHCDataItem, GetVideoDeviceDetailbyDeviceIdCallback, "json")
        .done(function (response) { UnBlockElement("divVideoHealthCheckContent"); })
        .fail(function (response) { UnBlockElement("divVideoHealthCheckContent"); });
}
function btnCancelLinkToAlarmClicked() {
    $("#viewMoreAlertsGridPopUp").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function ShowHelthcheckDescription() {
    $("#HelthcheckDescriptionwindow").data("kendoWindow").open().center();
}
function selectDevice(e) {
    var dataItem = { "DeviceId": this.dataItem(e.item.index()).Id };
    BlockElement("divVideoHealthCheckContent", 'Connecting...');
    $.post(videoHCGetPlatformDevicePollingStatusUrl, dataItem, GetVideoDeviceDetailbyDeviceIdCallback, "json");
}

function GetVideoDeviceDetailbyDeviceIdCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            $("#lblPollingStatus").text("");
            $("#lblDaysRecorded").text("");
            $("#lblCurrentlyRecording").text("");
            $("#lblStatus").text("");
            $("#videoalertlink").hide();
            fnShowAlertInfo(ResultSet.Message);
            UnBlockElement("divVideoHealthCheckContent");
        }
        else {
            $("#lblPollingStatus").text(ResultSet.PollingStatus);
            $("#lblDaysRecorded").text(ResultSet.DaysRecorded);
            $("#lblCurrentlyRecording").text(ResultSet.IsCurrentlyRecording);
            $("#lblStatus").text(ResultSet.Status);
            $("#videoalertlink").show();
        }
    }
    UnBlockElement("divVideoHealthCheckContent");
}
function fnCloseVideoHealthCheck() {
    var mySplitResultset = {
        'input': 'VideoHealthCheck'
    };
    $.post(videoHCClosePortletUrl, mySplitResultset, mySplitResultset, "json");
    $('#dvDisplayVideoHealthCheck').hide();
    getWidthofColumn();
    return false;
}

function fntoggleVideoHealthCheck() {
    $('#divVideoHealthCheckContent').toggle();
}

function GetDeviceTypes() {
    $.post(videoHCGetDevicesByUserUrl, null, GetDevicesByUserCallback, "json");
}

function GetDevicesByUserCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboDeviceList = $("#cmbDevicelist").data("kendoComboBox");
            cboDeviceList.dataSource.data(ResultSet);
            if (ResultSet != null && ResultSet.length > 0) {
                if (ResultSet != null && ResultSet.length > 0 && ResultSet[0].DefaultSelectedValue != undefined
                && ResultSet[0].DefaultSelectedValue != null
                    && ResultSet[0].DefaultSelectedValue != "") {
                    var selectedItem = ResultSet[0].DefaultSelectedValue;
                }
                if (selectedItem != null && selectedItem != undefined) {
                    cboDeviceList.value(selectedItem);
                    var dataItem = { "DeviceId": selectedItem };
                    BlockElement("divVideoHealthCheckContent", 'Connecting...');
                    $.post(videoHCGetPlatformDevicePollingStatusUrl, dataItem, GetVideoDeviceDetailbyDeviceIdCallback, "json");
                }
            }
        }
    }
}
function fnVideoHealthCheckMoreOptionsClick() {
    var pk = $("#cmbDevicelist").data("kendoComboBox").value();
    if (pk == '') {
        $("#VideoErrorPopup").data("kendoWindow").center().open();
    } else {
        var urlPath = videoHCShowStatusDeviceUrl.replace(0, pk);
        BlockElement("divVideoHealthCheckContent", 'Connecting...');
        $('#dvSettings').dialog('close');
        $.ajax({
            url: urlPath,
            type: 'POST',
            success: function (result) {
                UnBlockElement("divVideoHealthCheckContent");
                showDeviceStatusInformation(result);
            },
            error: function (xhr) {
                // showDialog("An error occurred while processing this action.", "Error");
                UnBlockElement("divVideoHealthCheckContent");
                var wnd = $("#Details").data("kendoWindow");
                wnd.content("<p style='height:100px'>An error occurred while processing this action.</p>");
                wnd.title("Error");
                wnd.center().open();
            }
        });
    }
}

function fnVHCRefreshClick() {
    var SelectedDeviceId = $("#cmbDevicelist").data("kendoComboBox").value();
    if (SelectedDeviceId != undefined && SelectedDeviceId != null && SelectedDeviceId != "") {
        BlockElement("divVideoHealthCheckContent", 'Connecting...');
        var VHCDataItem = { "deviceId": SelectedDeviceId };
        $.post(videoHCGetDevicePollingStatusUrl, VHCDataItem, GetVideoDeviceDetailbyDeviceIdCallback, "json")
        .done(function (response) { UnBlockElement("divVideoHealthCheckContent"); })
        .fail(function (response) { fnShowAlertInfo(response.Message); UnBlockElement("divVideoHealthCheckContent"); });
    }
    else {
        fnShowAlertInfo('Please select a device before refreshing.');
        return false;
    }
    jQuery('#dvSettings').dialog('close');
}

function fnAddDefaultValuesClick() {
    var parameters;
    var parameters = { "DeviceId": $("#cmbDevicelist").data("kendoComboBox").value(), "InternalName": "VIDEOHEALTHCHECK", "ControlName": "cmbDevicelist" };
    $.post(videoHCSaveDefaultValueUrl, parameters, null, "json");
}

function SaveDefaultValueCallback(ResultSet) {
    // Empty function for call back of save defaults
}
function fnClearVHCDefaultValuesClick() {
    var parameters;
    var parameters = { "InternalName": "VIDEOHEALTHCHECK", "ControlName": "cmbDevicelist" };
    $.post(videoHCClearVHCDefaultValueUrl, parameters, ClearVHCDefaultValueCallBack, "json");
}
function ClearVHCDefaultValueCallBack(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        fnShowAlertInfo(ResultSet);
    }
}

function displayGrid_VHCheck() {
    var DeviceId = $("#cmbDevicelist").data("kendoComboBox").value();

    var dataItem = { "DeviceId": DeviceId };
    $.post(videoHCGetActiveAlertsbyDeviceIdUrl, dataItem, GetActiveAlertsbyDeviceIdCallBack, "json");
}
function GetActiveAlertsbyDeviceIdCallBack(ResultSet) {
    centerViewPortDialog("#viewMoreAlertsGridPopUp");
    var grd2 = $("#LinkToAlarmGrid").data("kendoGrid");
    grd2.dataSource.data(ResultSet);
}
function openVideoheader() {
    if ($('#mainBody .k-animation-container #cmbDevicelist-list').length > 0) {
        if ($('#mainBody > #cmbDevicelist-list').length > 0) {
            $('#cmbDevicelist-list').remove();
        }
    }
    $('#cmbDevicelist-list').prepend("<div id='DDvideoheader'><div id='rowcell'>Device</div><div id='rowcell'>Location</div><div id='rowcell'>Address</div><div id='rowcell'>City</div><div id='rowcell'>State</div><div id='rowcell' style='border-right:0;'>Zip</div></li>");
}
function closeVideoheader() {
    $('#DDvideoheader').remove();
}
function showDeviceStatusInformation(result) {
    $('.k-overlay').remove();
    if (result != undefined)
        result = "<div id='dvDeviceStatusInfo'>" + result + "</div>";
    var wnd = $("#DeviceStatusInfo").data("kendoWindow");
    wnd.content(result);
    wnd.title("Current Readings");
    wnd.center().open();
}

// Live view 1

function Camera_SelectionChanged(e) {
    PlaySelectedCamera();
}

function Server_SelectionChanged(e) {
    var selectedServer = $("#ServerList").data("kendoComboBox").value();
    var dataSource = $('#CameraList').data().kendoComboBox.dataSource;

    var raw = dataSource.data();
    var length = raw.length;

    var item, i;
    for (i = length - 1; i >= 0; i--) {
        item = raw[i];
        dataSource.remove(item);
    }

    for (i = 0; i < serverDataSource.length; i++) {
        if (serverDataSource[i].ServerID == selectedServer) {
            dataSource.add(serverDataSource[i]);
        }
    }

    StopPlayer();
    var cmbCameraListData = $('#CameraList').data().kendoComboBox.dataSource.data();
    $('#CameraList').data().kendoComboBox.value("My Camera List");
    CurrentCamera = "";
}

function GetLiveViewDefaultValues() {
    var parameters = { "InternalName": "LIVEVIEW" };
    $.post(liveViewGetLIVEVIEWValueUrl, parameters, GetLIVEVIEWValueCallback, "json");
}

function GetLIVEVIEWValueCallback(ResultSet) {
    var cmbCamera = $('#CameraList').data().kendoComboBox;
    if (ResultSet != undefined && ResultSet != null) {
        var result = ResultSet.split("~");
        var cmbServerList = $('#ServerList').data().kendoComboBox;
        var cmbCameraList = $('#CameraList').data().kendoComboBox.dataSource;
        cmbServerList.value(result[0]);

        for (i = 0; i < serverDataSource.length; i++) {
            if (serverDataSource[i].LocationName == result[0]) {
                cmbCameraList.add(serverDataSource[i]);
            }
        }
        cmbCamera.value(result[1]);
        PlaySelectedCamera();
    }
    else {
        cmbCamera.value("My Camera List");
    }
}

function PlayCamera(camera) {
    CurrentCamera = camera;
    ipConfigure.Settings.ManagementUri = ManagementUri;
    player = new ipConfigure.Player(playerdiv);
    player.Play(CurrentCamera);
}

function StopPlayer() {
    player.Stop();
}
function Playback() {
    player.Play(CurrentCamera);
}

function PlaySelectedCamera() {
    var varSelectedValue = $("#CameraList").data("kendoComboBox").value();
    if (varSelectedValue > 0) {
        var cmbServerList = $('#CameraList').data().kendoComboBox;
        var datasource = cmbServerList.dataSource._data;
        var camera = null;
        for (var i = 0; i < datasource.length; i++) {
            if (parseInt(varSelectedValue) == datasource[i].CameraID) {
                camera = datasource[i];
            }
        }
        StopPlayer();
        PlayCamera(camera);
    }
    else {
        StopPlayer();
    }
}

function GetServerDetails() {
    ipConfigure.Settings.ManagementUri = ManagementUri;
    var cameras = $("#Cameras")
    var playerdiv = $("#ipConfigure-player");
    var player = new ipConfigure.Player(playerdiv);
    $.ajax(ipConfigure.Settings.ManagementUri + "/services/API.svc/GetCameras",
            { dataType: "jsonp" })
            .done(function (data) {
                data.d.sort(function (a, b) {
                    if (a.Name > b.Name) return 1;
                    else if (a.Name < b.Name) return -1;
                    else return 0;
                });
                serverDataSource = data.d;
                BindServerList(data.d)
            })
            .fail(function (data) { });
}

function fnLiveViewAddCamera() {
    $('#dvSettings').hide();
    $.post(liveViewAddNextCameraUrl, null, null, "json")
        .success(function (response) { window.location.reload(true); })
        .fail(function (response) { });
}

function fnLiveViewSaveDefaults() {
    $('#dvSettings').hide();
    var parameters = { "DefaultValue": $("#ServerList").data("kendoComboBox").text(), "InternalName": "LIVEVIEW", "ControlName": "ServerList" };
    $.post(liveViewSaveLiveViewDefaultsUrl, parameters, fnLiveViewSaveCallback, "json");
}

function fnLiveViewSaveCallback() {
    var parameters = { "DefaultValue": $("#CameraList").data("kendoComboBox").text(), "InternalName": "LIVEVIEW", "ControlName": "CameraList" };
    $.post(liveViewSaveLiveViewDefaultsUrl, parameters, null, "json");

}
function fnLiveViewClearDefaults() {
    $('#dvSettings').hide();
    var parameters = { "InternalName": "LIVEVIEW", "ControlName": "ServerList" };
    $.post(liveViewClearLiveViewDefaultsUrl, parameters, fnLiveViewClearCallback, "json");
}
function fnLiveViewClearCallback() {
    var parameters = { "InternalName": "LIVEVIEW", "ControlName": "CameraList" };
    $.post(liveViewClearLiveViewDefaultsUrl, parameters, ClearLiveViewDefaultsCallBack, "json");
}
function ClearLiveViewDefaultsCallBack(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        fnShowAlertInfo(ResultSet);
    }
}
function BindServerList(dataSource) {
    serverDataSource = dataSource;
    var cmbServerList = $('#ServerList').data().kendoComboBox;
    var usedNames = {};
    for (i = 0; i < dataSource.length; i++) {
        if (usedNames[dataSource[i].ServerID]) {

        } else {
            usedNames[dataSource[i].ServerID] = dataSource[i].LocationName;
            cmbServerList.dataSource.add(dataSource[i]);
        }
    }

    GetLiveViewDefaultValues();
}

function fnCloseLiveView() {
    var mySplitResultset = { 'input': 'LiveView' };
    $.post(liveViewClosePortletUrl, mySplitResultset, mySplitResultset, "json"); $('#dvLiveViewPortlet').hide(); getWidthofColumn();
    return false;
}
function fntoggleLiveView() {
    $('#dvLiveViewContent').toggle();
}
function ShowLiveViewDesc() {
    $("#LiveViewDescription").data("kendoWindow").center().open();
}

// live view 2

function GetDefaultValues() {
    var parameters = { "InternalName": "LIVEVIEW2" };
    $.post(liveViewGetLIVEVIEWValueTwoUrl, parameters, GetLiveViewTwoCallback, "json");
}

function GetLiveViewTwoCallback(ResultSet) {
    var cmbCameraTwo = $('#CameraListTwo').data().kendoComboBox;
    if (ResultSet != undefined && ResultSet != null) {
        var result = ResultSet.split("~");
        var cmbServerListTwo = $('#ServerListTwo').data().kendoComboBox;
        var cmbCameraListTwo = $('#CameraListTwo').data().kendoComboBox.dataSource;
        cmbServerListTwo.value(result[0]);

        for (i = 0; i < serverDataSourceTwo.length; i++) {
            if (serverDataSourceTwo[i].LocationName == result[0]) {
                cmbCameraListTwo.add(serverDataSourceTwo[i]);
            }
        }
        cmbCameraTwo.value(result[1]);
        PlaySelectedCameraTwo();
    }
    else {
        cmbCameraTwo.value("My Camera List");
    }
}

function Camera_SelectionChangedTwo(e) {
    //GetCameraDetailsTwo();
    PlaySelectedCameraTwo();
}

function Server_SelectionChangedTwo(e) {
    var selectedServer = $("#ServerListTwo").data("kendoComboBox").value();
    var dataSource = $('#CameraListTwo').data().kendoComboBox.dataSource;

    var raw = dataSource.data();
    var length = raw.length;

    var item, i;
    for (i = length - 1; i >= 0; i--) {
        item = raw[i];
        dataSource.remove(item);
    }

    for (i = 0; i < serverDataSourceTwo.length; i++) {
        if (serverDataSourceTwo[i].ServerID == selectedServer) {
            dataSource.add(serverDataSourceTwo[i]);
        }
    }
    StopPlayerTwo();
    var cmbCameraListData = $('#CameraListTwo').data().kendoComboBox.dataSource.data();
    $('#CameraListTwo').data().kendoComboBox.value("My Camera List");
    CurrentCameraTwo = "";
}
function PlayCameraTwo(camera) {
    CurrentCameraTwo = camera;
    ipConfigure.Settings.ManagementUri = ManagementUri;
    playerTwo = new ipConfigure.Player(playerdivTwo);
    playerTwo.Play(CurrentCameraTwo);
}

function StopPlayerTwo() {
    playerTwo.Stop();
}
function PlaybackTwo() {
    playerTwo.Play(CurrentCameraTwo);
}
function PlaySelectedCameraTwo() {
    var varSelectedValue = $("#CameraListTwo").data("kendoComboBox").value();
    if (varSelectedValue > 0) {
        var cmbServerList = $('#CameraListTwo').data().kendoComboBox;
        var datasource = cmbServerList.dataSource._data;
        var camera = null;
        for (var i = 0; i < datasource.length; i++) {
            if (parseInt(varSelectedValue) == datasource[i].CameraID) {
                camera = datasource[i];
            }
        }
        StopPlayerTwo();
        PlayCameraTwo(camera);
    }
    else {
        StopPlayerTwo();
    }
}

function GetServerDetailsTwo() {
    ipConfigure.Settings.ManagementUri = ManagementUri;
    var cameras = $("#Cameras")
    var playerdivTwo = $("#ipConfigure-playerTwo");
    var playerTwo = new ipConfigure.Player(playerdivTwo);

    $.ajax(ipConfigure.Settings.ManagementUri + "/services/API.svc/GetCameras",
                { dataType: "jsonp" })
                .done(function (data) {
                    data.d.sort(function (a, b) {
                        if (a.Name > b.Name) return 1;
                        else if (a.Name < b.Name) return -1;
                        else return 0;
                    });
                    serverDataSourceTwo = data.d;
                    BindServerListTwo(data.d)
                })
                .fail(function (data) {
                });
}

function BindServerListTwo(dataSource) {
    serverDataSourceTwo = dataSource;
    var cmbServerListTwo = $('#ServerListTwo').data().kendoComboBox;
    var usedNames = {};
    for (i = 0; i < dataSource.length; i++) {
        if (usedNames[dataSource[i].ServerID]) {

        } else {
            usedNames[dataSource[i].ServerID] = dataSource[i].LocationName;
            cmbServerListTwo.dataSource.add(dataSource[i]);
        }
    }
    GetDefaultValues();
}
function fnLiveViewTwoSaveDefaults() {
    $('#dvSettings').hide();
    var parameters = { "DefaultValue": $("#ServerListTwo").data("kendoComboBox").text(), "InternalName": "LIVEVIEW2", "ControlName": "ServerList" };
    $.post(liveViewSaveLiveViewDefaultsTwoUrl, parameters, fnLiveViewTwoSaveCallback, "json");
}

function fnLiveViewTwoSaveCallback() {
    var parameters = { "DefaultValue": $("#CameraListTwo").data("kendoComboBox").value(), "InternalName": "LIVEVIEW2", "ControlName": "CameraList" };
    $.post(liveViewSaveLiveViewDefaultsTwoUrl, parameters, null, "json");
}

function fnLiveViewTwoClearDefaults() {
    $('#dvSettings').hide();
    var parameters = { "InternalName": "LIVEVIEW2", "ControlName": "ServerList" };
    $.post(liveViewClearLiveViewDefaultsTwoUrl, parameters, fnLiveView2ClearCallback, "json");
}
function fnLiveView2ClearCallback() {
    var parameters = { "InternalName": "LIVEVIEW2", "ControlName": "CameraList" };
    $.post(liveViewClearLiveViewDefaultsTwoUrl, parameters, ClearLiveView2DefaultsCallBack, "json");
}
function ClearLiveView2DefaultsCallBack(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        fnShowAlertInfo(ResultSet);
    }
}

function fnCloseLiveViewTwo() {
    var mySplitResultset = { 'input': 'LIVEVIEW_TWO' };
    $.post(liveViewClosePortletTwoUrl, mySplitResultset, mySplitResultset, "json"); $('#dvLiveViewPortletTwo').hide(); getWidthofColumn();
    return false;
}
function fntoggleLiveViewTwo() {
    $('#dvLiveViewContentTwo').toggle();
}
function ShowLiveViewDescTwo() {
    $("#LiveViewDescription").data("kendoWindow").center().open();
}

// monitoring

function fnOpenMASDetails() {
    $.post(masGetAccountListforSearchUrl, null, GetMASSearchCallback, "json");
}
function GetMASSearchCallback(ResultSet) {
    var grdSearchMASDetails = $("#grdSearchMASDetails").data("kendoGrid");
    grdSearchMASDetails.dataSource.data(ResultSet);
    centerViewPortDialog("#dvSearchMASDevices");
}
function grdSearchMASChanged() {
    var selectedMASInfo = this.select();
    var accId = this.dataItem(selectedMASInfo).Id;
    var accName = this.dataItem(selectedMASInfo).AccountNumber;
    var cboMASAccountList = $("#MASAccountList").data("kendoComboBox");
    cboMASAccountList.value(accId);
    var selectedMASInfo = accId;
    MASAccountListChanged();
    $("#dvSearchMASDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function btnSearchMASCancelClicked() {
    $("#dvSearchMASDevices").css({ "display": "none" });

    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function btnCancelReportsClicked() {
    $("#dvReports").css({ "display": "none" });
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}

function btnCancelUserInfoClicked() {
    $("#dvUserPinValidate").css({ "display": "none" });
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}
function GetAccountDetailsList() {
    $.post(masGetAccountListUrl, null, GetAccountListCallback, "json");
}

function GetAccountListCallback(ResultSet) {
    var cboMASAccountList = $("#MASAccountList").data("kendoComboBox");
    cboMASAccountList.dataSource.data(ResultSet);
    if (ResultSet != null && ResultSet.length > 0 && ResultSet[0].DefaultSelectedValue != undefined
                && ResultSet[0].DefaultSelectedValue != null
                    && ResultSet[0].DefaultSelectedValue != "") {
        var selectedItem = ResultSet[0].DefaultSelectedValue;
        cboMASAccountList.value(selectedItem);
        SelectedSiteId = selectedItem;
    }
}

function GetTestDurationDetails() {
    $.post(masGetTestDurationUrl, null, GetTestDurationCallback, "json");
}

function GetTestDurationCallback(ResultSet) {
    if (ResultSet == 'No Action Available') {
        $("#btnPlaceonTest").attr("disabled", "disabled");
    }
    else {
        $("#btnPlaceonTest").removeAttr("disabled");
        var cboMASHoursList = $("#MASHoursList").data("kendoComboBox");
        cboMASHoursList.dataSource.data(ResultSet);
    }
}

function GetAllReportTypes() {
    $.post(masGetAllReportTypesUrl, null, GetReportTypeCallback, "json");
}

function GetReportTypeCallback(ResultSetReport) {
    var cboMASReportType = $("#MASReportType").data("kendoComboBox");
    cboMASReportType.dataSource.data(ResultSetReport);
    if (ResultSetReport != null && ResultSetReport.length > 0 && ResultSetReport[0].DefaultSelectedValue != undefined
                && ResultSetReport[0].DefaultSelectedValue != null
                    && ResultSetReport[0].DefaultSelectedValue != "") {
        var selectedReportItem = ResultSetReport[0].DefaultSelectedValue;
        cboMASReportType.value(selectedReportItem);

        var ReportName = ResultSetReport[0].DefaultSelectedValue;
        if (ReportName == "Call List" || ReportName == "Zone List" || ReportName == "Contact List") {
            document.getElementById('dvMASReportStartTime').style.visibility = "hidden";
            document.getElementById('dvMASReportEndTime').style.visibility = "hidden";
        }
        else {
            document.getElementById('dvMASReportStartTime').style.visibility = "visible";
            document.getElementById('dvMASReportEndTime').style.visibility = "visible";
        }
    }
}

function ReportTypeSelected(e) {
    // Call to server is made just to hold the selected value in session
    var ReportName = { "ReportName": this.dataItem(e.item.index()).Name };
    $.post(masSaveReportinSessionUrl, ReportName, null, "json");

    var ReportName = this.dataItem(e.item.index()).Name;
    if (ReportName == "Call List" || ReportName == "Zone List" || ReportName == "Contact List") {
        document.getElementById('dvMASReportStartTime').style.visibility = "hidden";
        document.getElementById('dvMASReportEndTime').style.visibility = "hidden";
    }
    else {
        document.getElementById('dvMASReportStartTime').style.visibility = "visible";
        document.getElementById('dvMASReportEndTime').style.visibility = "visible";
    }
}

function fnPlaceonTest() {
    // User Pin Validation
    var Resultset = {
        'SelectedSite': SelectedSiteId
    };
    var accName;
    var masaccountlistcombobox = $("#MASAccountList").data("kendoComboBox");
    var selectedMASInfo = masaccountlistcombobox.select();
    if (SelectedSiteId != 'undefined' && SelectedSiteId != null && SelectedSiteId != '') {
        accName = masaccountlistcombobox.dataItem(selectedMASInfo).AccountNumber;
    }  
    var btnPlaceonTestVal = $("#btnPlaceonTest").val();
    if (btnPlaceonTestVal == 'Clear Test') {
     
        var SelectedHrs = 0;
        var Resultset_Clear = {
            'SelectedSite': SelectedSiteId,
            'SelectedHour': SelectedHrs,
            'AccountNumber': accName
        };
        BlockElement("Monitoring", 'Connecting...');
        $.post(masPlaceonTestUrl, Resultset_Clear, PlaceonTestCallbackforClearTest, "json");
    }
    else {
        if (SelectedSiteId != 'undefined' && SelectedSiteId != null && SelectedSiteId != '') {
            $("#my_overlay").css({
                "display": "block",
                "visibility": "visible"
            });
            centerViewPortDialog('#dvUserPinValidate');
        }
        else {
            fnShowAlertInfo('Please select the account number before placing on test.');
        }
    }

}

function ConfirmPoT() {
    var UserPin = document.getElementById('txtAccessUserpinAccount').value;
    var SelectedHrs = $("#MASHoursList").data("kendoComboBox").value();
    GSelectedHours = SelectedHrs;
    var Resultset = {
        'UserPin': UserPin
    };
    if (UserPin != 'undefined' && UserPin != null && UserPin != '') {
        BlockElement("Monitoring", 'Connecting...');
        $.post(masValidateUserPinUrl, Resultset, ValidateUserPinCallback, "json");
    }
    else {
        fnShowAlertInfo('Please enter user pin');
        UnBlockElement("Monitoring");
    }
    var wnd = $("#WarningMessage").data("kendoWindow");
    wnd.close();
}

function CancelPoT() {
    var wnd = $("#WarningMessage").data("kendoWindow");
    wnd.close();
}

function btnValidateUserpinClicked() {
    var UserPin = document.getElementById('txtAccessUserpinAccount').value;
    var SelectedHrs = $("#MASHoursList").data("kendoComboBox").value();
    if (UserPin != 'undefined' && UserPin != null && UserPin != '' && UserPin.length > 3 && UserPin.length < 7) {
        if (SelectedHrs != 'undefined' && SelectedHrs != null && SelectedHrs != '') {
            var wnd = $("#WarningMessage").data("kendoWindow");
            wnd.content("<p>Important!</p>" +
                                                                                                                "<p style='margin-bottom:20%'>Please be aware you may be placing an account on test that contains fire detection. Certain jurisdictions or environments have laws or mandated procedures when placing an account on test that requires you to notify the local authority or provide a fire watch. Please contact your local authority if you are unsure how these apply. If you have any questions about placing an account on test please contact the Diebold monitoring center at 800-548-4478</p>" +
                                                                                                                "<p style='text-align:center;'>" +
                            "<input type='button' value='Ok' style='background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 10px 6px 10px;margin-right:4px;' onclick='ConfirmPoT()'>" +
                             "<input type='button' value='Cancel' style='background: none repeat scroll 0 0 #5E5E5E;border: medium none; color: #FFFFFF; cursor: pointer; font-size: 11px; font-weight: bold; margin: 0; padding: 6px 10px 6px 10px;margin-left:5px;' onclick='CancelPoT()'></p>");
            wnd.center().open();
        }
        else {
            fnShowAlertInfo('Please enter test duration');
        }
    }
    else {
        if (UserPin.length > 3 && UserPin.length < 7)
            fnShowAlertInfo('Please enter user pin');
        else
            fnShowAlertInfo('Please enter a valid user pin which accepts 4 to 6 digits');
    }
}

function ValidateUserPinCallback(Resultant) {
    document.getElementById('txtAccessUserpinAccount').value = "";
    if (Resultant != null && Resultant != undefined) {
        if (Resultant.Message != null && Resultant.Message != undefined && Resultant.Message != "") {
            fnShowAlertInfo(Resultant.Message);
            UnBlockElement("Monitoring");
        }
        else {
            if (Resultant == true) {
                centerViewPortDialog('#dvUserPinValidate');
                var accName;
                var masaccountlistcombobox = $("#MASAccountList").data("kendoComboBox");
                var selectedMASInfo = masaccountlistcombobox.select();
                if (SelectedSiteId != 'undefined' && SelectedSiteId != null && SelectedSiteId != '') {
                    accName = masaccountlistcombobox.dataItem(selectedMASInfo).AccountNumber;
                }
                var Resultset = {
                    'SelectedSite': SelectedSiteId,
                    'SelectedHour': GSelectedHours,
                    'AccountNumber': accName
                };
                if (SelectedSiteId != 'undefined' && SelectedSiteId != null && SelectedSiteId != '') {
                    if (GSelectedHours != 'undefined' && GSelectedHours != null && GSelectedHours != '') {
                        BlockElement("Monitoring", 'Connecting...');
                        $.post(masPlaceonTestUrl, Resultset, PlaceonTestCallback, "json");
                    }
                    else {
                        fnShowAlertInfo('Please select the test duration before placing on test.');
                        UnBlockElement("Monitoring");
                    }
                }
                else {
                    fnShowAlertInfo('Please select the account number before placing on test.');
                    UnBlockElement("Monitoring");
                    btnCancelUserInfoClicked();
                }
            }
            else if (Resultant == false) {
                fnShowAlertInfo('Invalid user pin');
                UnBlockElement("Monitoring");
            }
        }
    }
}

function PlaceonTestCallback(result) {
    if (result != null && result != undefined) {
        if (result.Message != null && result.Message != undefined && result.Message != "") {
            fnShowAlertInfo(result.Message);
            UnBlockElement("Monitoring");
        }
        else {
            UnBlockElement("Monitoring");
            btnCancelUserInfoClicked();
            fnShowAlertInfo(result);
            if (result == 'Test Successful.') {
                $("#btnPlaceonTest").val('Clear Test');
            }
        }
    }

}
function PlaceonTestCallbackforClearTest(result) {
    if (result != null && result != undefined) {
        if (result.Message != null && result.Message != undefined && result.Message != "") {
            fnShowAlertInfo(result.Message);
            UnBlockElement("Monitoring");
        }
        else {
            UnBlockElement("Monitoring");            
            if (result == 'Test Successful.') {
                result = 'Clear Successful'
                $("#btnPlaceonTest").val('Place on Test');
            }
            fnShowAlertInfo(result);
        }
    }

}
function PlaceonTestCallbackforDDChange(result) {   
    if (result != null && result != undefined) {
        if (result.Message != null && result.Message != undefined && result.Message != "") {
            fnShowAlertInfo(result.Message);          
            UnBlockElement("Monitoring");
        }
        else {
            if (result == 'Exists') {
                $("#btnPlaceonTest").val('Clear Test');
            }
            else if (result == 'NotExists') {
                $("#btnPlaceonTest").val('Place on Test');
            }
            UnBlockElement("Monitoring");           
        }
    }

}

function ShowMASDesc() {
    $("#MasDescription").data("kendoWindow").center().open();
}
function openMonitorDeviceList() {
    if ($('#mainBody .k-animation-container #MASAccountList-list').length > 0) {
        if ($('#mainBody > #MASAccountList-list').length > 0) {
            $('#MASAccountList-list').remove();
        }
    }
    $('#MASAccountList-list').prepend("<div id='DDMonitorheader'><div id='rowcell'>Acct No</div><div id='rowcell'>Site</div><div id='rowcell'>Address 1</div><div id='rowcell'>Address 2</div><div id='rowcell'>City</div><div id='rowcell'>State</div><div id='rowcell' style='border-right:0;'>Zip</div></div>");
}
function closeMonitorDeviceList() {
    $('#DDMonitorheader').remove();
}

var SelectedSiteId;
function MASAccountListChanged() {
    $("#btnPlaceonTest").val('Place on Test');
    SelectedSiteId = $("#MASAccountList").data("kendoComboBox").value();
    // Server Call is Made just to hold the selected value in session.
    var AccountNumber = { "AccountNumber": SelectedSiteId };
    $.post(masSaveAccountinSessionUrl, AccountNumber, null, "json");
    
    var accName;
    var masaccountlistcombobox = $("#MASAccountList").data("kendoComboBox");
    var selectedMASInfo = masaccountlistcombobox.select();
    if (SelectedSiteId != 'undefined' && SelectedSiteId != null && SelectedSiteId != '') {
        accName = masaccountlistcombobox.dataItem(selectedMASInfo).AccountNumber;
    }

    var SelectedHrs = 0;
    var Resultset = {
        'SelectedSite': SelectedSiteId,
        'SelectedHour': SelectedHrs,
        'AccountNumber': accName
    };
    BlockElement("Monitoring", 'Connecting...');
    $.post(masDDChangePlaceonTestUrl, Resultset, PlaceonTestCallbackforDDChange, "json");
}
var ReportType;
function fnRunReport() {
    var FromDate = $("#dpStartDate").val();
    var ToDate = $("#dpEndDate").val();
    ReportType = $("#MASReportType").data("kendoComboBox").text();
    var dtMorethanOneDay = new Date(Date.parse(FromDate));

    var dtFromDate = Date.parse(FromDate);
    var dtToDate = Date.parse(ToDate);

    var masaccountlistcombobox = $("#MASAccountList").data("kendoComboBox");
    var selectedMASInfo = masaccountlistcombobox.select();
    if (SelectedSiteId != 'undefined' && SelectedSiteId != null && SelectedSiteId != '') {
        var accName = masaccountlistcombobox.dataItem(selectedMASInfo).AccountNumber;
    }
    // var accName = masaccountlistcombobox.value();
    var Resultset = {
        'SelectedSite': masaccountlistcombobox.value()
    };

    if (SelectedSiteId != 'undefined' && SelectedSiteId != null && SelectedSiteId != '') {

    }
    else {
        fnShowAlertInfo('Please select the account number before running a report.');
        return false;
    }    
    if (ReportType != undefined && ReportType != null && ReportType != '' && ReportType != 'Report Type') {
        if (ReportType.toLowerCase() != 'zone list' && ReportType.toLowerCase() != 'contact list') {            
            if (dtToDate > dtFromDate) {
                if (dtToDate > dtMorethanOneDay.setDate(dtMorethanOneDay.getDate() + 3)) {
                    fnShowAlertInfo("Date difference between start and end date cannot be more than 3 days.");
                    return false;
                }
                else {
                    var RunReportDataItem = { "fromdate": FromDate, "todate": ToDate, "report": ReportType, "accountNumber": accName };
                    BlockElement("Monitoring", 'Connecting...');
                    $.post(masRunReportUrl, RunReportDataItem, runReportCallback, "json");
                }
            }
            else {
                fnShowAlertInfo("End date cannot be less than start date.");
                return false;
            }
        }
        else if (ReportType.toLowerCase() == 'zone list' || ReportType.toLowerCase() == 'contact list') {            
            var RunReportDataItem = { "fromdate": FromDate, "todate": ToDate, "report": ReportType, "accountNumber": accName };
            BlockElement("Monitoring", 'Connecting...');
            $.post(masRunReportUrl, RunReportDataItem, runReportCallback, "json");
        }
    }
    else {
        fnShowAlertInfo('Please select the report type.');
        return false;
    }
}

function runReportCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
            UnBlockElement("Monitoring");
            $("#my_overlay").css({
                "display": "none",
                "visibility": "hidden"
            });
        }
        else {
            UnBlockElement("Monitoring");
            $("#my_overlay").css({
                "display": "block",
                "visibility": "visible"
            });
            $("#lblMASReportType").val('');
            $("#lblMASReportType").text(ReportType + " ");       
            if (ReportType == 'Open / Close Normal' || ReportType == 'Open / Close Irregular' || ReportType == 'Events') {
                document.getElementById('dvAllReports').style.display = "block";
                document.getElementById('dvZoneListReport').style.display = "none";
                document.getElementById('dvContactListReport').style.display = "none";
                var grid = $("#grdReports").data("kendoGrid");
                //                    if (ReportType != 'Events') {
                //                        grid.hideColumn("Comment / Location");
                //                    }
                //                    else {
                //                        grid.showColumn("Comment / Location");
                //                    }
                // grid.dataSource.pageSize(5);
                grid.dataSource.data(ResultSet);
            }
            else if (ReportType == 'Zone List') {
                document.getElementById('dvAllReports').style.display = "none";
                document.getElementById('dvZoneListReport').style.display = "block";
                document.getElementById('dvContactListReport').style.display = "none";
                var grid = $("#grdZoneListReports").data("kendoGrid");
                // grid.dataSource.pageSize(5);
                grid.dataSource.data(ResultSet);

            }
            else {
                document.getElementById('dvAllReports').style.display = "none";
                document.getElementById('dvZoneListReport').style.display = "none";
                document.getElementById('dvContactListReport').style.display = "block";
                var grid = $("#grdContactListReports").data("kendoGrid");
                // grid.dataSource.pageSize(5);
                grid.dataSource.data(ResultSet);

            }
            centerViewPortDialog("#dvReports");
        }
    }
}
function fnCloseMas() {
    var mySplitResultset = {
        'input': 'MAS'
    };
    $.post(masClosePortletUrl, mySplitResultset, mySplitResultset, "json");

    $('#Monitoring').hide(); getWidthofColumn();
    return false;
}

function fntoggleMas() {
    $('#divMonitoringContent').toggle();
}
function fntoggleFireAlarm() {
    $('#FireAlarm-content').toggle();
}
function fnCloseFireAlarm() {
    var mySplitResultset = {
        'input': 'FireAlarm'
    };
    $.post(fireAlarmClosePortletUrl, mySplitResultset, mySplitResultset, "json");

    $('#FireAlarmportletArea').hide();
    getWidthofColumn();
    return false;
}

function GetSiteDetailsforFireAlarm() {
    $.post(GetSitesforFireWidgetUrl, null, GetSiteforFireWidgetCallback, "json");
}

function openDDFireAlarmheader() {
    if ($('#mainBody .k-animation-container #cboFireAlarmSite-list').length > 0) {
        if ($('#mainBody > #cboFireAlarmSite-list').length > 0) {
            $('#cboFireAlarmSite-list').remove();
        }
    }
    $('#cboFireAlarmSite-list').prepend("<div id='DDFireAlarmheader'><div class='rowcellheader'>Site</div><div class='rowcellheader' style='width:115px!important'>Account Number</div></div>");
}

function closeDDFireAlarmheader() {
   $('#DDFireAlarmheader').remove();
}

var FireAlarmDataItem;
function selectFireAlarmDevice(e) {
    BlockElement("FireAlarm-content", 'Connecting...');
    var combobox = $("#cboFireAlarmSite").data("kendoComboBox");
    FireAlarmDataItem = { "AccountNumber": combobox.value(), "SiteName": combobox.text() };
    $.post(GetPlatformFireAlarmUrl, FireAlarmDataItem, GetFireAlarmCallback, "json")
        .done(function (response) { UnBlockElement("FireAlarm-content"); })
        .fail(function (response) { UnBlockElement("FireAlarm-content"); });
}

function GetFireAlarmCallback(ResultSet) {
    UnBlockElement("FireAlarm-content");
    document.getElementById('lblAlarmCount').innerHTML = ResultSet.ALARM;
    document.getElementById('lblTroubleCount').innerHTML = ResultSet.TRBL;
    //        if (ResultSet.ALARM == 0 && ResultSet.TRBL == 0) {
    //            var wnd = $("#NoEvents").data("kendoWindow");
    //            wnd.content("<p style='height:20px'>No events available for Alarm.</p>" +
    //            "<p style='height:20px'>No events available for Trouble.</p>");
    //            wnd.center().open();
    //        }
    //        else if (ResultSet.ALARM == 0 && ResultSet.TRBL != 0) {
    //            var wnd = $("#NoEvents").data("kendoWindow");
    //            wnd.content("<p style='height:20px'>No events available for Alarm.</p>");
    //            wnd.center().open();
    //        }
    //        else if (ResultSet.ALARM != 0 && ResultSet.TRBL == 0) {
    //            var wnd = $("#NoEvents").data("kendoWindow");
    //            wnd.content("<p style='height:20px'>No events available for Trouble.</p>");
    //            wnd.center().open();
    //        }
}

function GetSiteforFireWidgetCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            var cboFireWidgetSite = $("#cboFireAlarmSite").data("kendoComboBox");
            cboFireWidgetSite.dataSource.data(ResultSet);
            if (ResultSet != null && ResultSet.length > 0 && ResultSet[0].DefaultSelectedValue != undefined
                && ResultSet[0].DefaultSelectedValue != null && ResultSet[0].DefaultSelectedValue != "") {
                var selectedItem = ResultSet[0].DefaultSelectedValue;
                if (selectedItem != null && selectedItem != undefined) {
                    cboFireWidgetSite.value(selectedItem);
                    // Make a call to getch the data and display.
                    BlockElement("FireAlarm-content", 'Connecting...');
                    FireAlarmDataItem = { "AccountNumber": cboFireWidgetSite.value(), "SiteName": cboFireWidgetSite.text() };
                    $.post(GetPlatformFireAlarmUrl, FireAlarmDataItem, GetFireAlarmCallback, "json")
                }
            }
        }
    }
}

function fnAddFADefaultValuesClick() {
    var parameters;
    var SelectedFireSite = $("#cboFireAlarmSite").data("kendoComboBox").text();
    if (SelectedFireSite == null || SelectedFireSite == undefined || SelectedFireSite == '') {
        fnShowAlertInfo('Please select a Site before saving it as default value');
    }
    else {
        var parameters = { "SiteName": SelectedFireSite, "InternalName": "FIREALARM", "ControlName": "cboFireAlarmSite" };
        $.post(FireAlarmSaveDefaultValueUrl, parameters, null, "json");

        jQuery('#dvSettings').dialog('close');
    }
}

function fnClearFADefaultValuesClick() {
    var parameters;
    var parameters = { "InternalName": "FIREALARM", "ControlName": "cboFireAlarmSite" };
    $.post(FireAlarmClearDefaultValueUrl, parameters, ClearFireAlarmDefaultValueCallBack, "json");
}
function ClearFireAlarmDefaultValueCallBack(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        fnShowAlertInfo(ResultSet);
    }
}

function ShowFirePortletDescription() {
    $("#FireWidgetDescriptionwindow").data("kendoWindow").open().center();
}

function fnshowFireEvents() {
    BlockElement("FireAlarm-content", 'Connecting...');
    var combobox = $("#cboFireAlarmSite").data("kendoComboBox");
    if (combobox.value() != null && combobox.value() != undefined && combobox.value() != '') {
        FireAlarmDataItem = { "AccountNumber": combobox.value(), "SiteName": combobox.text() };
        $.post(GetPlatformFireAlarmEventUrl, FireAlarmDataItem, GetFireAlarmEventCallback, "json")
            .done(function (response) { UnBlockElement("FireAlarm-content"); })
            .fail(function (response) { UnBlockElement("FireAlarm-content"); });
    }
    else {
        fnShowAlertInfo('Please select a Site before seeing the details.');
        UnBlockElement("FireAlarm-content");
    }
}
function GetFireAlarmEventCallback(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
            fnShowAlertInfo(ResultSet.Message);
        }
        else {
            centerViewPortDialog("#dvFireEvents");
            var grid = $("#grdFireWidgetEvents").data("kendoGrid");
            grid.dataSource.data(ResultSet);
        }
    }
    else {
        centerViewPortDialog("#dvFireEvents");
        $("#grdFireWidgetEvents").data("kendoGrid").dataSource.data([]);
    }
}
function fnAddSiteMapDefaultValuesClick() {
    var parameters = { "SiteId": $("#dvSiteId").text(), "InternalName": "SITEMAP", "ControlName": "lblSiteMap" };
    $.post(siteMapSaveDefaultValueUrl, parameters, null, "json");
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}
function fnClearSiteMapDefaultValuesClick() {
    var parameters = { "InternalName": "SITEMAP", "ControlName": "lblSiteMap" };
    $.post(siteMapClearDefaultValueUrl, parameters, ClearSiteMapDefaultValueCallBack, "json");
}
function ClearSiteMapDefaultValueCallBack(ResultSet) {
    if (ResultSet != null && ResultSet != undefined) {
        fnShowAlertInfo(ResultSet);
    }
    if (jQuery('#dvSettings').dialog('isOpen')) {
        jQuery('#dvSettings').dialog('close');
    }
}
function LoadingDefaultFilter() {
    setTimeout(function () {
        $('.k-header').each(function () {
            if ($(this).data('kendoColumnMenu')) {
                var header = $(this).data('kendoColumnMenu');
                if (header.filterMenu) {
                    header.filterMenu.popup.bind('open', function () {
                        if (!$(this.element).data('alreadyOpened')) {
                            var select = this.element.find('select:first');
                            var option = select.children('option:contains("Contains")');
                            if (option.length > 0) {
                                header.filterMenu.filterModel.filters[0].operator = "contains";
                                select.data('kendoDropDownList').select(option.index());
                            }
                            $(this.element).data('alreadyOpened', true);
                        }
                    });
                    header.filterMenu.form.bind('reset', function () {
                        $(this).parent().data('kendoFilterMenu').popup.element.data('alreadyOpened', false);
                    });
                }
            } else if ($(this).data('kendoFilterMenu')) {
                var header = $(this).data('kendoFilterMenu');
                header.popup.bind('open', function () {
                    if (!$(this.element).data('alreadyOpened')) {
                        var select = this.element.find('select:first');
                        var option = select.children('option:contains("Contains")');
                        if (option.length > 0) {
                            header.filterModel.filters[0].operator = "contains";
                            select.data('kendoDropDownList').select(option.index());
                        }
                        $(this.element).data('alreadyOpened', true);
                    }
                    else {
                        var select = this.element.find('select:first');
                        if (header.filterModel.filters[0].operator == 'eq') {
                            var select = this.element.find('select:first');
                            var option = select.children('option:contains("Contains")');
                            if (option.length > 0) {
                                header.filterModel.filters[0].operator = "contains";
                                select.data('kendoDropDownList').select(option.index());
                            }
                        }
                    }
                });
                header.form.bind('reset', function () {
                    header.popup.element.data('alreadyOpened', false);
                });
            }
        });
    }, 1);

}

function btnCancelFireEventClicked() {
    $("#dvFireEvents").css({ "display": "none" });
    $("#my_overlay").css({
        "display": "none",
        "visibility": "hidden"
    });
}