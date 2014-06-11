
$(document).ready(function () {

    $('.step1').show();

    $(".chzn-select").chosen();

    $('.btnStep1').live('click', function () {

        if ($('.step2').is(':visible')) {
            if ($('.GroupingRelations_' + $('#Grouping1LevelName').val() + '_Grouping1Id').length > 0 || DdlContainsText('#Grouping1Selected', $('#Grouping1LevelName').val())) {
                alreadyExistsDialog('company grouping');
            }
            else {
                if ($('#Grouping1LevelName').val() != '') {
                    AddNewG1ElementTo('#Grouping1Selected', '.Grouping1ItemSelected', '#Grouping1LevelName');
                }
                HideStep();
                $('.step1').show();
            }
        }
        else {

            var elementValue = $('#Grouping2LevelName').val();
            if (elementValue != '') {

                if ($('.GroupingRelations_' + $('#Grouping1Selected option:selected').val() + '_Grouping2List_' + $('#Grouping2LevelName').val() + '_Grouping2Id').length > 0
                    || DdlContainsText('#groupingLevel2ItemsToValidate', $('#Grouping2LevelName').val()) || DdlContainsText('#Grouping2Selected', $('#Grouping2LevelName').val())) {
                    alreadyExistsDialog('company grouping');
                }
                else {
                    var e = $.Event();
                    e.type = 'new';
                    e.value = elementValue;
                    e.text = elementValue;

                    $('#Grouping2SelectedItems').trigger('change', e);

                    HideStep();
                    $('.step1').show();
                }
            }
            else {
                HideStep();
                $('.step1').show();
            }
        }
        $('#Grouping1Selected').trigger("liszt:updated");
    });

    $('.btnStep2').live('click', function () {
        HideStep();
        $('.step2').show();
    });

    $('.btnStep3').live('click', function () {
        HideStep();
        $('.step3').show();
    });

    $(".editableInfo").hide();
    $("#groupingLevel2ItemsToValidate").hide();

    $('#anotherGroupLevel1').live('click', function () {
        if ($('.GroupingRelations_' + $('#Grouping1LevelName').val() + '_Grouping1Id').length > 0 || DdlContainsText('#Grouping1Selected', $('#Grouping1LevelName').val())) {
            alreadyExistsDialog('company grouping');
        }
        else {
            AddNewG1ElementTo('#Grouping1Selected', '.Grouping1ItemSelected', '#Grouping1LevelName');
        }
    });

    $('#anotherGroupLevel2').live('click', function () {
        var elementValue = $('#Grouping2LevelName').val();

        if ($('form').attr("action") != "/Company/Create") {

            $(".Grouping1ItemSelected").change();

            if ($('.GroupingRelations_' + $('#Grouping1Selected option:selected').val() + '_Grouping2List_' + $('#Grouping2LevelName').val() + '_Grouping2Id').length > 0
                    || DdlContainsText('#groupingLevel2ItemsToValidate', $('#Grouping2LevelName').val()) || DdlContainsText('#Grouping2Selected', $('#Grouping2LevelName').val())) {
                alreadyExistsDialog('company grouping');
            }
            else {
                var e = $.Event();
                e.type = 'new';
                e.value = elementValue;
                e.text = elementValue;

                $('#Grouping2SelectedItems').trigger('change', e);
            }
        }
        else {
            if ($('.GroupingRelations_' + $('#Grouping1Selected option:selected').val() + '_Grouping2List_' + $('#Grouping2LevelName').val() + '_Grouping2Id').length > 0) {
                alreadyExistsDialog('company grouping');
            }
            else {
                AddNewG2ElementTo($('#Grouping1Selected option:selected').val(), $('#Grouping1Selected option:selected').text(), null, null, elementValue, elementValue, false);
                $('#Grouping2LevelName').val('');
            }
        }

    });

    //$("#groupingLevel2ItemsToValidate").CascadingDropDown(".Grouping1ItemSelected", '/Company/AsyncGrouping2Levels', {
       // promptText: null,
       // postData: function () {
        //    return { group1LavelId: $('#Grouping1Selected').val(), companyId: $('#Id').val() };
        //}
    //});

//    $("#Grouping2Selected").CascadingDropDown("#Grouping1Selected", '/Company/AsyncGrouping2Levels',
//                    {
//                        promptText: null,
//                        postData: function () {
//                            return { group1LevelId: $('#Grouping1Selected').val(), companyId: $('#Id').val() };
//                        },
//                        onLoaded: function (data) {
//                            $('#Grouping2Selected').trigger("liszt:updated");

//                            $('#Grouping2SelectedItems option:selected').each(function () {
//                                $(this).removeAttr('selected');
//                            });

//                            $('#Grouping2SelectedItems option').each(function () {
//                                if ($("#Grouping2Selected option[value='" + $(this).val() + "']").length > 0) {
//                                    $(this).attr('selected', 'selected');
//                                }
//                            });

//                            var grouping1Selected = GetCustomValue($('#Grouping1Selected').val());

//                            $('input.GroupingRelations_' + grouping1Selected + '_Grouping2List').each(function () {
//                                var grouping2Id = GetCustomValue($(this).val());
//                                var grouping2Name = $('.GroupingRelations_' + grouping1Selected + '_Grouping2List_' + grouping2Id + '_Grouping2Name').val();
//                                var isRemoved = $('.GroupingRelations_' + grouping1Selected + '_Grouping2List_' + grouping2Id + '_IsRemoved').val();

//                                var toRemove = false;

//                                if (isRemoved != undefined && isRemoved == 'true') {
//                                    toRemove = true;
//                                }

//                                if (toRemove) {
//                                    $("#Grouping2SelectedItems option[value='" + grouping2Id + "']").removeAttr('selected');
//                                    $("#Grouping2Selected option[value='" + grouping2Id + "']").remove();
//                                }
//                                else {
//                                    $("#Grouping2SelectedItems option[value='" + grouping2Id + "']").attr('selected', 'selected');
//                                    $('#Grouping2Selected').append($("<option></option>").attr("value", grouping2Id).text(grouping2Name));
//                                }
//                            });

//                            $("#Grouping2SelectedItems").trigger("change");
//                            $("#Grouping2SelectedItems").bsmSelect();

//                            // Chosen refresh
//                            $('#Grouping2Selected').trigger("liszt:updated");
//                        }
//                    });
                    
    
    $("#Grouping2Selected").change(function () {
        var grouping1Value = $('#Grouping1Selected').val();
        var grouping2Value = $('#Grouping2Selected').val();

        if (grouping2Value != null) {
            $.ajax({
                url: '/Company/AsyncSites',
                type: 'POST',
                dataType: 'json',
                data: { group2LavelId: grouping2Value, group1LavelId: grouping1Value, companyId: $('#Id').val() },
                success: function (data) {
                    RemoveSites();
                    $.each(data, function (key, val) {
                        var item = $("#Sites option[value='" + data[key].Value + "']");
                        $(item).attr('selected', 'selected');
                    });

                    $('.GroupingRelations_' + grouping1Value + '_Grouping2List_' + grouping2Value + '_Sites').each(function () {
                        var siteId = $('.GroupingRelations_' + grouping1Value + '_Grouping2List_' + grouping2Value + '_Sites_' + $(this).val() + '_SiteId').val();
                        var isRemoved = $('.GroupingRelations_' + grouping1Value + '_Grouping2List_' + grouping2Value + '_Sites_' + $(this).val() + '_IsRemoved').val();

                        var toRemove = false;

                        if (isRemoved != undefined && isRemoved == 'true') {
                            toRemove = true;
                        }

                        if (toRemove) {
                            $("#Sites option[value='" + siteId + "']").removeAttr('selected');
                        }
                        else {
                            $("#Sites option[value='" + siteId + "']").attr('selected', 'selected');
                        }

                    });

                    UpdateSites();
                }
            });
        }
        else {
            RemoveSites();
            UpdateSites();
        }
    });
    
    
    $("#Grouping2SelectedItems").change(function (sender, e) {

        if (e != undefined) {
            if (e.type == 'add') {
                AddNewG2ElementTo($('#Grouping1Selected option:selected').val(), $('#Grouping1Selected option:selected').text(), $('#Grouping2Selected option:selected'), $('#Grouping2SelectedItems option:selected'), e.value, e.option.text(), false);
                $('#Grouping2Selected').append($("<option></option>").attr("value", e.value).text(e.option.text()), false);
            }
            else if (e.type == 'drop') {

                if (e.text == undefined)
                    e.text = $(e.option)[0].label;
                
                RemoveG2ElementTo(e, $('#Grouping1Selected option:selected').val(), $('#Grouping1Selected option:selected').text(), $('#Grouping2Selected option:selected'), $('#Grouping2SelectedItems option:selected'), "", true);
            }
            else if (e.type == 'new') {
                AddNewG2ElementTo($('.Grouping1ItemSelected option:selected').val(), $('.Grouping1ItemSelected option:selected').text(), '#Grouping2Selected', '#Grouping2ItemSelected', e.value, e.value, false);
                if ($('#Grouping1Selected option:selected').val() == $('.Grouping1ItemSelected option:selected').val()) {
                    $('#Grouping2SelectedItems').append($("<option></option>").attr('selected', 'selected').attr("value", e.value).text(e.text));
                    $('#Grouping2Selected').append($("<option></option>").attr("value", e.value).text(e.text));
                }
                $('#Grouping2LevelName').val('');
            }

            // Chosen refresh
            $('#Grouping2Selected').trigger("liszt:updated");
        }


    });
    
    
    $("#Sites").change(function (sender, e) {
        if (e != undefined) {
            if (e.type == 'add') {
                AddNewSiteElementTo(e, false);
            }
            else if (e.type == 'drop') {
                RemoveSiteElementTo(e, $('#Grouping1Selected option:selected').val(), $('#Grouping2Selected option:selected').val());
            }
        }
    });
    
    //$(".chzn-select").chosen();
    //$(".multiselect").bsmSelect();
});

function AppendInput(element, elementValue, container, cssClass, attribute, index) {
    var input = $('<input/>').attr('name', element).attr('type', 'hidden');

    if (cssClass != null) {
        $(input).attr('class', cssClass);
    }

    if (index != null) {
        $(input).text(index);
        if (attribute != null) {
            $(input).attr(attribute, index);
        }
    }

    $(input).val(elementValue);
    $(container).append(input);
}

function AddNewG1ElementTo(ddlG1Control, ddlG1ItemControl, element) {
    if ($(element).val() != '') {
        
        if (!ExistsItemInDropDownList(ddlG1Control, $(element).val())) {

            var grouping1Value = $(element).val();
            var grouping1Index = GetGrouping1NextIndex();
            var cssClassG1 = 'GroupingRelations' + '_' + GetCustomValue(grouping1Value) + '_' + 'Grouping1Id';
            AddG1Element(grouping1Index, grouping1Value, '[' + grouping1Index + '].', grouping1Value, 'GroupingRelations', cssClassG1);

            //Refresh District Dropdownlist.
            $(ddlG1Control).append($("<option></option>").attr("value", $(element).val()).text($(element).val()));
            //Refresh District dropdownlist in create area.
            $(ddlG1ItemControl).append($("<option></option>").attr("value", $(element).val()).text($(element).val()));

            $(element).val('');
        }
    }
    else {
        inputFieldEmptyDialog('company grouping');
    }
}

function AddNewG2ElementTo(ddlG1Value, ddlG1Name, ddlG2Control, ddlG2ItemControl, elementValue, elementText, isRemoved) {
    if (elementValue != '') {
        if (!ExistsItemInDropDownList(ddlG2Control, elementValue)) {
            var grouping1Index = GetGrouping1Index(ddlG1Value);
            if (grouping1Index == "") {
                grouping1Index = GetGrouping1NextIndex();
                var cssClassG1 = 'GroupingRelations' + '_' + GetCustomValue(ddlG1Value) + '_' + 'Grouping1Id';
                AddG1Element(grouping1Index, ddlG1Value, '[' + grouping1Index + '].', ddlG1Name, 'GroupingRelations', cssClassG1);
            }
            var grouping1Prefix = '[' + grouping1Index + '].';

            var grouping2Value = elementValue;
            var grouping2Index = GetGrouping2NextIndex(ddlG1Value);
            var cssClassG2 = 'GroupingRelations_' + GetCustomValue(ddlG1Value) + '_Grouping2List_' + GetCustomValue(grouping2Value);
            var grouping2Prefix = '[' + grouping2Index + '].';
            AddG2Element(ddlG1Value, grouping1Prefix, grouping2Index, grouping2Value, elementText, grouping2Prefix, 'GroupingRelations', isRemoved, cssClassG2);
        }
    }
    else {
        inputFieldEmptyDialog('company grouping');
    }
}

//Create grouping1 element
function AddG1Element(grouping1Index, grouping1Value, grouping1Prefix, grouping1Name, elementRelation, cssClass) {
    AppendInput(elementRelation + grouping1Prefix + 'Grouping1Id', grouping1Value, '.groupingRelationContainer',
                        cssClass + ' GroupingRelations_Grouping1', null, grouping1Index);
    AppendInput(elementRelation + grouping1Prefix + 'Grouping1Name', grouping1Name, '.groupingRelationContainer', null);
}

//Create grouping2 element
function AddG2Element(grouping1Value, grouping1Prefix, grouping2Index, grouping2Value, grouping2Name, grouping2Prefix, elementRelation, isRemoved, cssClass) {

    if (isRemoved == undefined)
        isRemoved = false;

    AppendInput(elementRelation + grouping1Prefix + 'Grouping2List' + grouping2Prefix + 'Grouping2Id', grouping2Value, '.groupingRelationContainer',
        cssClass + '_Grouping2Id' + ' GroupingRelations_' + GetCustomValue(grouping1Value) + '_Grouping2List', null, grouping2Index);
    AppendInput(elementRelation + grouping1Prefix + 'Grouping2List' + grouping2Prefix + 'Grouping2Name', grouping2Name, '.groupingRelationContainer',
        cssClass + '_Grouping2Name');
    AppendInput(elementRelation + grouping1Prefix + 'Grouping2List' + grouping2Prefix + 'IsRemoved', isRemoved, '.groupingRelationContainer', cssClass + '_IsRemoved');
}

//Create site element
function AddSiteElement(grouping1Value, grouping1Prefix, grouping2Value, grouping2Prefix, siteIndex, siteValue, sitePrefix, siteText, elementRelation, isRemoved, cssClass) {
    AppendInput('GroupingRelations' + grouping1Prefix + 'Grouping2List' + grouping2Prefix + 'Sites' + sitePrefix + 'SiteId', siteValue, '.groupingRelationContainer',
                cssClass + '_SiteId' + ' GroupingRelations_' + GetCustomValue(grouping1Value) + '_Grouping2List_' + GetCustomValue(grouping2Value) + '_Sites', null, siteIndex);
    AppendInput('GroupingRelations' + grouping1Prefix + 'Grouping2List' + grouping2Prefix + 'Sites' + sitePrefix + 'SiteName', siteText, '.groupingRelationContainer', cssClass + '_SiteName');
    AppendInput('GroupingRelations' + grouping1Prefix + 'Grouping2List' + grouping2Prefix + 'Sites' + sitePrefix + 'IsRemoved', isRemoved, '.groupingRelationContainer', cssClass + '_IsRemoved');
}

function AddNewSiteElementTo(e, isRemoved) {

    var grouping1Value = $('#Grouping1Selected option:selected').val();
    var grouping1Name = $('#Grouping1Selected option:selected').text();
    var grouping2Value = $('#Grouping2Selected option:selected').val();

    //Creating grouping1 to add grouping2 elements
    var grouping1Index = GetGrouping1Index(grouping1Value);
    if (grouping1Index == "") {
        grouping1Index = GetGrouping1NextIndex();
        var cssClassG1 = 'GroupingRelations' + '_' + GetCustomValue(grouping1Value) + '_' + 'Grouping1Id';
        AddG1Element(grouping1Index, grouping1Value, '[' + grouping1Index + '].', grouping1Name, 'GroupingRelations', cssClassG1);
    }
    var grouping1Prefix = '[' + grouping1Index + '].';
    //End creating grouping1

    //Creating grouping2 to add site elements
    var grouping2Index = GetGrouping2Index(grouping1Value, grouping2Value);
    if (grouping2Index == "") {
        grouping2Index = GetGrouping2NextIndex(grouping1Value);
        var cssClassG2 = 'GroupingRelations_' + GetCustomValue(grouping1Value) + '_Grouping2List_' + GetCustomValue(grouping2Value);
        AddG2Element(grouping1Value, grouping1Prefix, grouping2Index, grouping2Value, GetCustomValue(grouping2Value), '[' + grouping2Index + '].', 'GroupingRelations', false, cssClassG2);
    }
    var grouping2Prefix = '[' + grouping2Index + '].';
    //End creating grouping2

    var siteIndex = GetSiteIndex(grouping1Value, grouping2Value, e.value);
    if (siteIndex == "") {
        siteIndex = GetSiteNextIndex(grouping1Value, grouping2Value);
        var cssClassSite = 'GroupingRelations_' + grouping1Value + '_Grouping2List_' + grouping2Value + '_Sites_' + e.value;
        AddSiteElement(grouping1Value, grouping1Prefix, grouping2Value, grouping2Prefix, siteIndex, e.value,
                               '[' + siteIndex + '].', e.option.text(), 'GroupingRelations', isRemoved, cssClassSite);
    }
}

function RemoveSiteElementTo(e, ddlG1Value, ddlG2Value) {

    var siteIndex = GetSiteIndex(ddlG1Value, ddlG2Value, e.value);
    var cssClass = 'GroupingRelations_' + ddlG1Value + '_Grouping2List_' + ddlG2Value + '_Sites_' + GetCustomValue(e.value);

    if (siteIndex != '') {
        $('.' + cssClass + '_IsRemoved').val(true);
    }
    else {
        AddNewSiteElementTo(e, true);
    }
}

function RemoveG2ElementTo(e, ddlG1Value, ddlG1Name, ddlG2Control, ddlG2ItemControl) {

    var grouping2Index = GetGrouping2Index(ddlG1Value, e.value);
    var cssClass = 'GroupingRelations_' + ddlG1Value + '_Grouping2List_' + GetCustomValue(e.value);

    if (grouping2Index != '') {
        $('.' + cssClass + '_IsRemoved').val(true);
    }
    else {
        AddNewG2ElementTo(ddlG1Value, ddlG1Name, ddlG2Control, ddlG2ItemControl, e.value, e.text, true);
    }

    $("#Grouping2Selected option[value='" + e.value + "']").remove();
    $("#Grouping2Selected").trigger("change");
}

function RemoveSites() {
    $('#Sites option:selected').each(function () {
        $(this).removeAttr('selected');
    });
}

function UpdateSites() {
    $("#Sites").trigger("change");
    $("#Sites").bsmSelect();
}

function HideStep() {
    $(".wizard-step:visible").hide();
}

function ExistsItemInDropDownList(element, itemValue) {
    var exists = false;
    $(element).each(function () {
        if ($(this).text().toLowerCase() == itemValue.toLowerCase()) {
            exists = true;
        }

    });
    return exists;
}

function DdlContainsText(element, itemValue) {
    var exists = false;

    $(element + ' option').each(function () {
        if ($(this).text().toLowerCase() == itemValue.toLowerCase()) {
            exists = true;
        }
    });

    return exists;
}

function DdlContainsValue(ddlControl, value) {
    return (($(ddlControl).find(":contains('" + value + "')").length > 0) || ($(ddlControl).find(":contains('" + value.toLowerCase() + "')").length > 0));
}

function ExistsGroupingElement(cssClass) {
    return ($('.' + cssClass).length > 0);
}

function GetGrouping1NextIndex() {
    return $('input.GroupingRelations_Grouping1').length;
}

function GetGrouping1Index(grouping1Value) {
    return $('input.GroupingRelations_' + GetCustomValue(grouping1Value) + '_Grouping1Id').text();
}

function GetGrouping2NextIndex(grouping1Value) {
    return $('input.GroupingRelations_' + GetCustomValue(grouping1Value) + '_Grouping2List').length;
}

function GetGrouping2Index(grouping1Value, grouping2Value) {
    return $('input.GroupingRelations_' + GetCustomValue(grouping1Value) + '_Grouping2List_' + GetCustomValue(grouping2Value) + '_Grouping2Id').text();
}

function GetSiteNextIndex(grouping1Value, grouping2Value) {
    return $('input.GroupingRelations_' + GetCustomValue(grouping1Value) + '_Grouping2List_' + GetCustomValue(grouping2Value) + '_Sites').length;
}

function GetSiteIndex(grouping1Value, grouping2Value, siteValue) {
    return $('input.GroupingRelations_' + GetCustomValue(grouping1Value) + '_Grouping2List_' + GetCustomValue(grouping2Value) + '_Sites_' + GetCustomValue(siteValue) + '_SiteId').text();
}

function GetCustomValue(elementValue) {
    return elementValue.replace(/ /g, '_');
}
