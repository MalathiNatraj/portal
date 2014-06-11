(function (Diebold, $, undefined) {
    var _deviceId, _devices;
    Diebold.Monitoring = {
        getMASMedia: function (masId) {
            var parameters = { id: masId, serverId: 1 };
            var modal = $('.pemodal').data('modal');
            var content = $('.modalContent');
            
            content.html("<style type=\"text/css\">.centeredImage { text-align:center;margin-top:259%;margin-left:400%;padding:0px;}</style><p class=\"centeredImage\"><img src=\"\\content\\images\\loading.gif\"\\></p>");
            modal.open();
            $.ajax({
                url: "/Monitor/GetMASMedia",
                data: parameters,
                dataType: "html",
                type: 'GET',
                success: function (data) {
                    //test data
                    //obj = { "MediaType": "Video", "MediaId": "33793", "Title": "", "Status": "Completed", "Description": "", "Notes": "mediaType: video\r\n\r\nProcessing Video Media\r\n\r\nC:\\Frontel2\\bin\\FrontelVideoConv.exe\r\n\r\n\"C:\\Videofied_Store\\video\\33793.mp4\" /oid=33793 /v=mp4 /DBHost=10.79.15.35\r\n\r\n\r\nCAPTURE_SUCCESS\r\n\r\n", "FileName": "/media/video/33793.mp4", "CreatedDate": "\/Date(1379467670671)\/", "Id": 83656704 };

                    // console.log(data);
                    obj = JSON.parse(data);
                    var filename = obj.FileName;
                    var status = obj.Status;
                    if (status != null && status.toUpperCase() == "COMPLETED") {
                        content.html("<div class=\"videoInfo\">Click <a href=\"" + filename + "\" target=\"_blank\">here</a> to view your " + obj.MediaType.toLowerCase() + ". You may also download the image by right-clicking.</div>");
                    } else {
                       content.html('<div>Could not retrieve media</div>');
                    }

                    UnBlockElement("Intrusion-content");
                }
            });
            return false;
        }
    }
}(window.Diebold = window.Diebold || {}, jQuery));
