
/******************************************************************************************************************************************/
/* Events */
/******************************************************************************************************************************************/

$(document).ready(function () {

    $('#anotherGroupLevel1').val('Add');
    $('#anotherGroupLevel2').val('Add');

    $('.step1').show();
    $(".chzn-select").chosen();

    $('#btnFinishLevel1').live('click', function () {
        showMain();
    });

    $('#btnFinishLevel2').live('click', function () {
        showMain();
    });

    $('.btnStep2').live('click', function () {
        showDistrictModule();
    });

    $('.btnStep3').live('click', function () {
        showAreaModule();
    });

    $(".editableInfo").hide();
    $("#groupingLevel2ItemsToValidate").hide();

    //$(".multiselect").bsmSelect();
    //$(".siteMultiselect").bsmSelect({ addRemoveLink: false });

    $("#Grouping2Selected").CascadingDropDown("#Grouping1Selected", '/Company/AsyncGrouping2Levels',
        {
            promptText: null,
            postData: function () {
                return { group1LevelId: $('#Grouping1Selected').val(), companyId: $('#Id').val() };
            },
            onLoaded: function () {
                refreshAreas();
            },
            onReseted: function () {
                refreshAreas();
            }
        });

    $("#Grouping2Selected").change(function () {

        EnableDisableSiteDDL();

        postPartial("/Company/AsyncSites", loadLevelParams(), true, null, onSuccessRefreshSites);
    });

    $('#anotherGroupLevel1').live('click', function () {
        addLevel1();
    });

    $('#anotherGroupLevel2').live('click', function () {
        addLevel2();
    });

    $("#Sites").change(function (sender, e) {
        if (e != undefined) {
            if (e.type == 'add') {
                //confirmDialog('<div id="confirmDialog" title="Site"> <p>The site will be reassigned to the selected area </p> <p>Do you want to continue?</p></div>', false);
                postPartial("/Company/AsyncReasignSite", loadReasignSiteParams(sender, e), true, ".rightPanel");
            }
        }
    });

    $('#btnEditDistrict').live('click', function () {

        $('#GroupingLevel1Name').val($('#Grouping1Selected option:selected').text());
        $('#groupingLevel1EditName').show();
        showEditDialog($('#groupingLevel1EditName'), $('#FirstLevelGrouping').val() + " name edition", "District", $('#groupingLevel1EditName'));
    });

    $('#btnEditArea').live('click', function () {
        $('#GroupingLevel2Name').val($('#Grouping2Selected option:selected').text());
        $('#groupingLevel2EditName').show();
        showEditDialog($('#groupingLevel2EditName'), $('#SecondLevelGrouping').val() + " name edition", "Area", $('#groupingLevel2EditName'));
    });

    $("#Grouping2SelectedItems").change(function (sender, e) {
        if (e != undefined) {
            if (e.type == 'add') {
                postPartial("/Company/AsyncReasignArea", loadReasignAreaParams(sender, e), true, ".rightPanel", onSuccessReasignArea);
            }
            else if (e.type == 'drop') {
                if (e.option.parent()[0].length == 1) {
                    showDialog("Atlease one area should be define.", "Validation Error");
                    refreshAreas();
                    return false;
                }
                else {
                    postPartial("/Company/AsyncRemoveGrouping2Level", loadReasignAreaParams(sender, e), true, ".rightPanel", onSuccessRemoveArea);
                }
            }
        }
    });

    $("#Grouping1SelectedItems").change(function (sender, e) {
        if (e != undefined) {
            if (e.type == 'drop') {
                postPartial("/Company/AsyncRemoveGrouping1Level", loadReasignDistrictParams(sender, e), true, ".rightPanel", onSuccessRemoveDistrict);
            }
        }
    });

    EnableDisableSiteDDL();
});

/******************************************************************************************************************************************/
/* Add Elements */
/******************************************************************************************************************************************/

function addLevel1(onComplete) {
    if (!ExistsDistrictInCompany($('#Grouping1LevelName').val())) {
        postPartial("/Company/AsyncCreateGroupingLevel1", loadLevelParams(), true, "#editcomgrouping", onSuccessAddsLevel1, onComplete);
    }
    else {
        alreadyExistsDialog('district');
    }
}

function addLevel2(onComplete) {
    postPartial("/Company/AsyncCreateGroupingLevel2", loadLevelParams(), true, ".rightPanel", onSuccessAddsLevel2, onComplete);

}

/******************************************************************************************************************************************/
/* Event Handlers */
/******************************************************************************************************************************************/

function onSuccessAddsLevel1(result) {
    if (result.levelId != undefined && result.levelName != undefined) {
        $('#Grouping1SelectedItems').append($("<option></option>").attr("selected", "selected").attr("value", result.levelId).text(result.levelName)).change();
        $('#Grouping1Selected').append($("<option></option>").attr("value", result.levelId).text(result.levelName));
        $('#Grouping1SelectedStep').append($("<option></option>").attr("value", result.levelId).text(result.levelName));
        $('#Grouping1LevelName').val('');
        $("#Grouping1SelectedStep").trigger("change");

        refreshGroupingLevel1Items();
    }
}

function onSuccessAddsLevel2(result) {

    if (result.levelId != undefined && result.levelName != undefined) {
        $('#Grouping2Selected').append($("<option></option>").attr("value", result.levelId).text(result.levelName)); //All items

        if ($('#Grouping1Selected option:selected').val() == $('#Grouping1SelectedStep option:selected').val()) {
            $('#Grouping2SelectedItems').append($("<option></option>").attr("value", result.levelId).attr('selected', 'selected').text(result.levelNameComplete));
        }
        else {
            $('#Grouping2SelectedItems').append($("<option></option>").attr("value", result.levelId).text(result.levelNameComplete));
        }

        $('#Grouping2LevelName').val('');

        refreshGroupingLevel2Items();
    }
}

function onSuccessRefreshSites(result) {
    updateSites(result);
}

function onSuccessChangeLevel1Name(result) {
    $('#GroupingLevel1Name').val('');
    $("#Grouping1Selected option[value='" + result.levelId + "']").text(result.levelName);
    $("#Grouping1SelectedStep option[value='" + result.levelId + "']").text(result.levelName);
    $("#Grouping1SelectedItems").trigger("change");

    refreshGroupingLevel1Items();

    $.each(result.level2Items, function (index) {
        var item = $("#Grouping2SelectedItems option[value='" + result.level2Items[index].Data.levelId + "']");
        $(item).text(result.level2Items[index].Data.levelName);
    });
    
    $("#Grouping2SelectedItems").trigger("change");
}

function onSuccessChangeLevel2Name(result) {
    $('#GroupingLevel2Name').val('');
    $("#Grouping2Selected option[value='" + result.levelId + "']").text(result.levelName);
    $("#Grouping2SelectedItems option[value='" + result.levelId + "']").text(result.levelNameComplete);

    refreshGroupingLevel2Items();
}

function onSuccessReasignArea(result) {
    $("#Grouping2SelectedItems option[value='" + result.levelId + "']").text(result.levelNameComplete);
    $('#Grouping2Selected').append($("<option></option>").attr("value", result.levelId).text(result.levelName)).change();

    refreshGroupingLevel2Items();
}

function onComplete() {
    showMain();
}

function onCompleteChangeLevel1Name() {
    $('#groupingLevel1EditName').hide();
}

function onCompleteChangeLevel2Name() {
    $('#groupingLevel2EditName').hide();
}

function onSuccessRemoveDistrict(result) {
    if (result.Status == "Error") {
        $("#Grouping1SelectedItems option[value='" + result.levelId + "']").attr("selected", "selected").change();
    }
    else {
        $("#Grouping1Selected option[value='" + result.levelId + "']").remove();
        $("#Grouping1SelectedStep option[value='" + result.levelId + "']").remove();
        $('#Grouping1Selected').trigger("liszt:updated");
    }
}

function onSuccessRemoveArea(result) {
    if (result.Status == "Error") {
        $("#Grouping2SelectedItems option[value='" + result.levelId + "']").attr("selected", "selected").change();
    }
    else {        
        $("#Grouping2Selected option[value='" + result.levelId + "']").remove();
        $("#Grouping2SelectedItems option[value='" + result.levelId + "']").remove();

        refreshGroupingLevel2Items();
    }
}

/******************************************************************************************************************************************/
/* Load Parameters */
/******************************************************************************************************************************************/

function loadLevelParams() {
    var companyId = $('#Id').val();
    var groupingLevel1Id = $('#Grouping1SelectedStep option:selected').val();
    var groupingLevel1Name = $('#Grouping1LevelName').val();
    var groupingLevel2Id = $('#Grouping2Selected option:selected').val();
    var groupingLevel2Name = $('#Grouping2LevelName').val();

    return { companyId: companyId, GroupingLevel2Name: groupingLevel2Name, GroupingLevel1Id: groupingLevel1Id,
            GroupingLevel2Id: groupingLevel2Id, GroupingLevel1Name: groupingLevel1Name};
}

function loadReasignSiteParams(sender, e) {
    var companyId = $('#Id').val();
    var groupingLevel2Id = $('#Grouping2Selected option:selected').val();
    var siteId = e.value;

    return { CompanyId: companyId, GroupingLevel2Id: groupingLevel2Id, SiteId: siteId };
}

function loadReasignAreaParams(sender, e) {
    var companyId = $('#Id').val();
    var groupingLevel1Id = $('#Grouping1Selected option:selected').val();
    var groupingLevel2Id = e.value; 

    return { CompanyId: companyId,  GroupingLevel1Id: groupingLevel1Id, GroupingLevel2Id: groupingLevel2Id };
}

function loadReasignDistrictParams(sender, e) {
    var companyId = $('#Id').val();
    var groupingLevel1Id = e.value;

    return { CompanyId: companyId, GroupingLevel1Id: groupingLevel1Id };
}

function loadRenameParams() {
    var companyId = $('#Id').val();
    var groupingLevel1Id = $('#Grouping1Selected option:selected').val();
    var groupingLevel1Name = $('#GroupingLevel1Name').val();
    var groupingLevel2Id = $('#Grouping2Selected option:selected').val();
    var groupingLevel2Name =  $('#GroupingLevel2Name').val();

    return { companyId: companyId, GroupingLevel1Id: groupingLevel1Id, GroupingLevel2Id: groupingLevel2Id, GroupingLevel1Name: groupingLevel1Name,
             GroupingLevel2Name: groupingLevel2Name };

}

/******************************************************************************************************************************************/
/* Generic Functions */
/******************************************************************************************************************************************/

function postAreaName(params) {
    postPartial("/Company/AsyncChangeLevel2Name", params, true, "#groupingNameEdition", onSuccessChangeLevel2Name, onCompleteChangeLevel2Name);
}

function postDistrictName(params) {
    postPartial("/Company/AsyncChangeLevel1Name", params, true, "#groupingNameEdition", onSuccessChangeLevel1Name, onCompleteChangeLevel1Name);
}

function refreshGroupingLevel1Items() {
    $("#Grouping1SelectedItems").bsmSelect();
    $('#Grouping1Selected').trigger("liszt:updated");
}

function refreshGroupingLevel2Items() {
    $("#Grouping2SelectedItems").trigger("change");
    $("#Grouping2SelectedItems").bsmSelect();
    $('#Grouping2Selected').trigger("liszt:updated");
}

function hideStep() {
    $(".wizard-step:visible").hide();
}

    function showMain() {
    hideStep();
    $('.step1').show();
}

function showDistrictModule() {
    hideStep();
    $('.step2').show();
}

function showAreaModule() {
    hideStep();
    $('.step3').show();
}

function EnableDisableSiteDDL() {
    if ($("#Grouping2Selected option").size() > 0)
        $("#bsmSelectbsmContainer2").removeAttr("disabled");
    else
        $("#bsmSelectbsmContainer2").attr("disabled", "disabled");
}

function refreshAreas() {
    $('#Grouping2SelectedItems option:selected').each(function () {
        $(this).removeAttr('selected');
    });

    $('#Grouping2SelectedItems option').each(function () {
        if ($("#Grouping2Selected option[value='" + $(this).val() + "']").length > 0) {
            $(this).attr('selected', 'selected');
        }
    });

    $("#Grouping2SelectedItems").trigger("change");
    $("#Grouping2SelectedItems").bsmSelect();
    $('#Grouping2Selected').trigger("liszt:updated");
}

function updateSites(result) {
    $('#Sites option:selected').each(function () {
        $(this).removeAttr('selected');
    });

    $.each(result, function (index) {
        var item = $("#Sites option[value='" + result[index].Value + "']");
        $(item).attr('selected', 'selected');
    });
    
    $("#Sites").trigger("change");
    $("#Sites").bsmSelect();
}

function ExistsDistrictInCompany(itemValue) {
    var exists = false;
    
    $('#Grouping1Selected option').each(function () {
        if ($(this).text().toLowerCase() == itemValue.toLowerCase()) {
            exists = true;
        }
    });
    
    return exists;
}

function showEditDialog(dialogContent, title, functionToRun, panelToHide) {

    $(dialogContent).dialog({
        autoOpen: true,
        resizable: false,
        title: (title != undefined) ? title : '',
        height: 140,
        modal: true,
        buttons: {
            "Save": function () {

                if ($(this).find("input").val().trim().length > 32) {
                    showDialog("The name must be a string with a maximum length of 32", "Validation Error");
                    
                    return false;
                }

                if (functionToRun != undefined) {
                    if (functionToRun == "Area") {
                        $('#GroupingLevel2Name').val($(this).find("input").val().trim());
                        postAreaName(loadRenameParams());
                    }
                    else if (functionToRun == "District")
                        $('#GroupingLevel1Name').val($(this).find("input").val().trim());
                    postDistrictName(loadRenameParams());
                }

                $(panelToHide).hide();

                $(this).dialog("close");

            },
            Cancel: function () {
                $(this).dialog("close");
                $(panelToHide).hide();
            }
        }
    });
}