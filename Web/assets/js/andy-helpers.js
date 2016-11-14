var _blankTable = new Array();
var _loadShowing = false;
var _redirectIe = false;
$(document).ready(function () {
    
});
function loadingModal() {
    _loadShowing = true;
    $.fancybox.open({        
        padding: 10,
        href: 'assets/js/loading.html',
        width: 200,
        height: 100,
        autoSize: true,
        type: 'iframe',
        modal: true
    });
}
function closeLoading() {
    if (_loadShowing) { $.fancybox.close(); }
}
function splitPipeIntoInts(combinedString) {
    return combinedString.split("|");
}
function isNull(data) {
    if (data === null || typeof (data) === 'undefined')
        return true;
    else
        return false;
}

function ajaxHandler(_url, _type, successFunction, postData, extraData, completeFunction) {
    try {        
        if (_type.toUpperCase() === 'GET') {
            $.ajax({
                type: _type,
                datatype: 'json',
                url: _url,
                data: postData,
                success: function (data) { successFunction(data, extraData); },
                error: function () { closeLoading(); alert('unable to call ' + _url + ' and return valid JSON results'); },
                complete: function () { if (!isNull) { completeFunction(); } }
            });
        }
        if (_type.toUpperCase() === 'POST') {
            if (isNull(postData)) {
                alert('ajaxHandler error = POST method but no data provided');
                return;
            }
            $.ajax({
                type: _type,
                datatype: 'json',
                data: JSON.stringify(postData),
                url: _url,
                success: function (data) { successFunction(data, extraData); },
                error: function () { closeLoading(); alert('unable to call ' + _url + ' and return valid JSON results'); },
                complete: function () { if (!isNull) { completeFunction(); } }
            });
        }        
    } catch (e) { alert('ajaxHandler error = ' + e) }
}
function msieversion() {
    var ua = window.navigator.userAgent
    var msie = ua.indexOf("MSIE ")
    if (msie > 0)      // If Internet Explorer, return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf(".", msie)))
    else                 // If another browser, return 0
        return 0
}
function notify(i, msg) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-top-center",
        "onclick": null,
        "showDuration": "1000",
        "hideDuration": "1000",
        "timeOut": "2000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    var type = i.toUpperCase();
    switch (type) {
        case "SUCCESS":
            toastr.success(msg);
            break;
        case "INFO":
            toastr.info(msg);
            break;
        case "WARNING":
            toastr.warning(msg);
            break;
        case "ERROR":
            toastr.error(msg);
            break;
        default:
            toastr.info(msg);
            break;
    }
    return false;
}
function parseDate(mask, value) {
    try{

        var returnDate = null;
        var _day = 0;
        var _month = 0;
        var _year = 0;
        var _hour = 0;
        var _min = 0;
        var _sec = 0;
        switch (mask) {
            case "dd/MM/yyyy HH:mm:ss":
                _day = parseInt(value.substr(0, 2));
                _month = parseInt(value.substr(3, 2));
                _year = parseInt(value.substr(6, 4));
                _hour = parseInt(value.substr(11, 2));
                _min = parseInt(value.substr(14, 2));
                _sec = parseInt(value.substr(17, 2));
                break;
            case "dd/MM/yyyy HH:mm":
                _day = parseInt(value.substr(0, 2));
                _month = parseInt(value.substr(3, 2));
                _year = parseInt(value.substr(6, 4));
                _hour = parseInt(value.substr(11, 2));
                _min = parseInt(value.substr(14, 2));
                break;
            case "dd/MM/yyyy":
                _day = parseInt(value.substr(0, 2));
                _month = parseInt(value.substr(3, 2));
                _year = parseInt(value.substr(6, 4));
                break;
        }
        returnDate = new Date(_year, _month - 1, _day, _hour, _min, _sec);
        //alert(_day + "\n" + _month + "\n" + _year + "\n" + _hour + "\n" + _min);
        return returnDate;

    } catch (e) { alert(e.message); return null; }    
}

function isValidDate(d) {
    if (Object.prototype.toString.call(d) === "[object Date]") {
        // it is a date
        if (isNaN(d.getTime())) {  // d.valueOf() could also work
            // date is not valid
            return false;
        }
        else {
            // date is valid
            return true;
        }
    }
    else {
        // not a date
        return false;
    }
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function setSelectedValue(selectId, valueToSet) {
    var selectObj = document.getElementById(selectId);
    for (var i = 0; i < selectObj.options.length; i++) {
        if (selectObj.options[i].text == valueToSet) {
            selectObj.options[i].selected = true;
            return;
        }
    }
}
function clearSelect(selectid) {
    var selectbox = document.getElementById(selectid);
    var i;
    for (i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.remove(i);
    }
}
function clearRadios(radioName) {
    var ele = document.getElementsByName(radioName);
    for (var i = 0; i < ele.length; i++)
        ele[i].checked = false;
}

function setSelectToNothing(selectid) {
    try {
        var elements = document.getElementById(selectid);
        for (i = 0; i < elements.length; i++) {
            elements[i].selectedIndex = -1;
        }
    } catch (e) { alert(e.message); }
}

//DT HELPER FUNCTIONS
function dtDrawTable(tblData, tblName) {
    try {
        var _tempDt = $('#' + tblName).DataTable();
        _tempDt.clear();
        _tempDt.rows.add(tblData.Data).draw();
        
    } catch (e) { alert(e.message); }
}

function dtSetupFooter(tblName) {
    try {
        var aTable = $('#' + tblName).DataTable();

        // Setup - add a text input to each footer cell
        $('#' + tblName + ' tfoot td').each(function () {
            if ($(this).hasClass('details-control')) {
                $(this).removeClass('details-control');
                return true;
            }
            var title = $('#' + tblName + ' thead th').eq($(this).index()).text();
            $(this).html('<input type="text" class="form-control" />');
        });

        // Apply the search
        aTable.columns().eq(0).each(function (colIdx) {
            $('input', aTable.column(colIdx).footer()).on('keyup change', function () {
                aTable
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
        });
    } catch (e) { alert(e); }
}

function dtSetupDetailsClick(tblName, formatEntries, openFunction, closeFunction) {
    try {

        var aTable = $('#' + tblName).DataTable();
        // Add event listener for opening and closing details
        $('#' + tblName + ' tbody').on('click', 'td.details-control', function () {
            try {
                var tr = $(this).closest('tr');
                var row = aTable.row(tr);
                if (row.child.isShown()) {
                    // This row is already open - close it
                    row.child.hide();
                    tr.removeClass('shown');
                    if (!isNull(closeFunction)) { closeFunction(tr); }                    
                }
                else {
                    // Open this row
                    row.child(formatEntries(row.data()), 'dt_childrow').show();
                    tr.addClass('shown');
                    if (!isNull(openFunction)) { openFunction(tr); }
                }
            }
            catch (e) {
                alert(e);
            }
        });

    } catch (e) { alert(e); }

}

function dtSetupRowClick1(tblName, openFunction, closeFunction) {
    try {        
        var _dt = $('#' + tblName).DataTable();
        $('#' + tblName + ' tbody').on('click', 'td', function () {
            //alert($(this).closest('tr').attr('class'));
            if ($(this).closest('tr').hasClass('dt_childrow')) { return; }
            if ($(this).parents().hasClass('dt_childrow')) { return; }
            var tr = $(this).closest('tr');
            var rowData = _dt.row(tr).data();
            var td = $(this).closest('td');

            //alert();

            if ($(td).hasClass('details-control')) {
                //alert('fired');
                return false;
            }

            //alert(JSON.stringify(rowData));
            if ($(tr).hasClass('row_selected')) {
                $(tr).removeClass('row_selected');
                closeFunction(rowData);
            }
            else {
                _dt.$('tr.row_selected').removeClass('row_selected');
                $(tr).addClass('row_selected');
                openFunction(rowData);
            }
        });

    } catch (e) { alert(e); }
}

function dtSetupRowClick12(tblName, openFunction, closeFunction) {
    try {
        var _dt = $('#' + tblName).DataTable();
        $('#' + tblName + ' tbody').on('click', 'tr', function () {
            var rowData = _dt.row(this).data();
            if ($(this).hasClass('row_selected')) {
                $(this).removeClass('row_selected');
                closeFunction(rowData);
            }
            else {
                _dt.$('tr.row_selected').removeClass('row_selected');
                $(this).addClass('row_selected');
                openFunction(rowData);
            }
        });
    } catch (e) { alert(e); }
}

function dtSetupRowCheckboxClick(tblName, openFunction, closeFunction) {
    try {
        var _dt = $('#' + tblName).DataTable();
        $('#' + tblName + ' tbody').on('click', 'input[type="checkbox"]', function (e) {
            var tr = $(this).closest('tr');
            var tbl = $(this).closest('table').attr('id');
            var rowData = _dt.row(tr).data();
            if (tbl == tblName) {
                if (this.checked) {
                    $(tr).addClass('row_selected');
                    openFunction(rowData, tr);
                } else {
                    $(tr).removeClass('row_selected');
                    closeFunction(rowData, tr);
                }
            }
        });
    } catch (e) { alert(e); }
}