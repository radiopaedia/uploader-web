<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Radiopaedia Uploader</title>

    <!-- Bootstrap -->
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/plugins/fancybox/jquery.fancybox.css" rel="stylesheet" />
    <link href="assets/plugins/toastr/build/toastr.min.css" rel="stylesheet" />
    <!-- DataTables -->
    <link href="assets/plugins/datatables/css/dataTables.bootstrap.css" rel="stylesheet" />
    <link href="assets/js/style2.css" rel="stylesheet" />


    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->

    <script src="assets/js/jquery.min.js"></script>
    <script src="assets/js/bootstrap.min.js"></script>
    <script src="assets/js/purl.js"></script>
    <script src="assets/plugins/fancybox/jquery.fancybox.js"></script>
    <script src="assets/plugins/toastr/build/toastr.min.js"></script>
    <script src="assets/js/date.js"></script>
    <script src="assets/plugins/datatables/js/jquery.dataTables.min.js"></script>
    <script src="assets/plugins/datatables/js/dataTables.bootstrap.min.js"></script>
    <script src="assets/js/andy-helpers.js"></script>


</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <h1 class="pull-right">Radiopaedia Uploader | Home</h1>
                </div>
            </div>
            <br />
            <br />
            <div id="div_content" style="display: none">
                <label id="lbl_welcome"></label>
                <br />
                <div class="row">
                    <div class="col-md-12">
                        <b>Your Quotas On Radiopaedia:</b>
                        <table class="table table-bordered">
                            <thead>
                                <tr>
                                    <th>Draft Cases</th>
                                    <th>Unlisted Cases</th>
                                    <th>Unlisted Playlists</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td id="td_drafts"></td>
                                    <td id="td_unlisted"></td>
                                    <td id="td_playlists"></td>
                                </tr>
                            </tbody>
                        </table>                        
                        <button class="btn btn-default pull-right" onclick="refreshQuotas(); return false;"><i class="glyphicon glyphicon-refresh"></i> Refresh Quotas</button>                      
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-md-12">
                        <%--<pre>Use the buttons below to navigate, do not press Refresh or Reload on your web browser!</pre>--%>
                        
                        <button class="btn btn-primary" onclick="addnew(); return false;" id="btn_add"><i class="glyphicon glyphicon-plus"></i> Upload New</button>                        
                        <br /><br />
                        <button class="btn" onclick="showCompleted(); return false;" id="btn_completed""><i class="glyphicon glyphicon-check"></i> Completed</button>
                        <button class="btn" onclick="showPending(); return false;" id="btn_pending"><i class="glyphicon glyphicon-cloud-upload"></i> Pending</button>                        
                        <button class="btn" onclick="showErrors(); return false;" id="btn_errors"><i class="glyphicon glyphicon-alert"></i> Errors</button>
                        <button class="btn btn-default"><i class="glyphicon glyphicon-refresh" onclick="showCompleted(); return false;"></i> Refresh Data</button>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-md-12">
                        <div class="panel panel-success">
                            <div class="panel-heading" id="lbl_tableTitle">

                            </div>
                            <div class="panel-body">
                                <table class="table table-bordered" id="tbl_cases">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th>Request Date</th>
                                            <th>Case Title</th>
                                            <th>Status</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>
                        </div> 
                        <br />
                        <button class="btn btn-default" id="btn_events" onclick="showEvents(); return false;"> <i class="glyphicon glyphicon-eye-open"></i> View Events Log For Case</button>
                        <button class="btn btn-default" id="btn_hide_events" onclick="hideEvents(); return false;" style="display:none"> <i class="glyphicon glyphicon-eye-close"></i> Hide Events</button>                                                                      
                    </div>                    
                </div>                
                <br />               
                <div class="row" id="div_events">
                    <div class="col-md-12">
                        <div class="panel panel-default">
                            <div class="panel-heading" id="lbl_events">
                                Events For Case
                            </div>
                            <div class="panel-body">
                                <table class="table table-bordered" id="tbl_events">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th>Timetstamp</th>
                                            <th>Type</th>
                                            <th>Message</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>
                        </div>                        
                    </div>
                </div>
            </div>            
        </div>
        <asp:HiddenField ID="hid_username" runat="server" />
        <asp:HiddenField ID="hid_auth" runat="server" />
        <asp:HiddenField ID="hid_token" runat="server" />
        <asp:HiddenField ID="hid_refresh" runat="server" />
    </form>




    <script>
        var refresh = document.getElementById('<%= hid_refresh.ClientID%>').value;
        var auth = document.getElementById('<%= hid_auth.ClientID%>').value;
        var username = document.getElementById('<%= hid_username.ClientID%>').value;
        var token = document.getElementById('<%= hid_token.ClientID%>').value;
        var quotas = null;
        $(document).ready(function () {
            if (auth.toUpperCase() == "TRUE") {
                $('#lbl_welcome').text("Welcome " + username + ' [' + refresh + ']');
                $('#div_auth').hide();
                $('#div_content').show();
                $('#div_user').hide();
                $('#div_user2').hide();
                ajaxHandler('Handlers/GetTable.ashx?status=Completed&username=' + username, 'get', postGetTable);
                ajaxHandler('Handlers/GetUserDetailsFromApi.ashx?token=' + token, 'get', getQuotas);
            } else { return false; }
            $('#btn_events').prop('disabled', true);
            $('#div_events').hide();
            var _blank = new Array();
            $('#tbl_cases').DataTable({
                "destroy": true,
                "dom": "<'row' <'col-md-12'B>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",
                "data": _blank,
                "orderClasses": false,
                "order": [
                    [1, 'desc']
                ],
                "columns": [
                    {
                        "class": 'details-control',
                        "orderable": false,
                        "data": null,
                        "defaultContent": ''
                    },
                    {
                        "data": "date", "render": function (data, type, full, meta) {
                            if (type === 'display' || type === 'filter') { var javascriptDate = new Date(data); return javascriptDate.toString("dd/MM/yyyy HH:mm"); } return data;
                        },
                        "width": "20%"
                    },
                    { "data": "title" },
                    {
                        "data": "status_message", "render": function (data, type, full, meta) {
                            var retString = data;
                            if (data.indexOf('Case fully uploaded: ') > -1) {
                                var split = data.split(": ");
                                retString = split[0] + '\n <a href="' + split[1] + '" target="_blank">' + split[1] + '</a>';
                                return retString;
                            }
                            return chunk(retString, 20).join(" ");
                        }
                    }
                ]
            });
            dtSetupRowClick1('tbl_cases', caseSelect, caseDeselect);
            dtSetupDetailsClick('tbl_cases', expandDetails);
            $('#tbl_events').DataTable({
                "pageLength": 5,
                "destroy": true,
                "dom": "<'row' <'col-md-12'B>><'row'<'col-md-6 col-sm-12'><'col-md-6 col-sm-12'>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",
                "data": _blank,
                "orderClasses": false,
                "order": [
                    [1, 'desc']
                ],
                "columns": [
                    {
                        "class": 'details-control',
                        "orderable": false,
                        "data": null,
                        "defaultContent": ''
                    },
                    {
                        "data": "TimeStamp", "render": function (data, type, full, meta) {
                            if (type === 'display' || type === 'filter') { var javascriptDate = new Date(data); return javascriptDate.toString("dd/MM/yyyy HH:mm:ss"); } return data;
                        }
                    },
                    { "data": "Type" },
                    { "data": "Message" }
                ]
            });
            dtSetupDetailsClick('tbl_events', expandedEvents);
            showCompleted();
        });
        function refreshQuotas() { ajaxHandler('Handlers/GetUserDetailsFromApi.ashx?token=' + token, 'get', getQuotas); }
        function getQuotas(data) {
            if (!data.Success) {
                notify('error', data.Message);
                return false;
            }
            quotas = data.Data.quotas;
            $('#td_drafts').html(quotas.draft_case_count + '/' + quotas.allowed_draft_cases);
            $('#td_unlisted').html(quotas.unlisted_case_count + '/' + quotas.allowed_unlisted_cases);
            $('#td_playlists').html(quotas.unlisted_playlist_count + '/' + quotas.allowed_unlisted_playlists);
            if (isNull(quotas.allowed_draft_cases) || quotas.allowed_draft_cases == 0) {
                $('#td_drafts').html(quotas.draft_case_count + '/&infin;');
            }
            if (isNull(quotas.allowed_draft_cases) || quotas.allowed_draft_cases == 0) {
                $('#td_unlisted').html(quotas.unlisted_case_count + '/&infin;');
            }
            if (isNull(quotas.allowed_draft_cases) || quotas.allowed_draft_cases == 0) {
                $('#td_playlists').html(quotas.unlisted_playlist_count + '/&infin;');
            }
            if (quotas.draft_case_count >= quotas.allowed_draft_cases) {
                $('#td_drafts').html('<font:"color=red">' + $('#td_drafts').html() + '</font>');
                $('#btn_add').prop('disabled', true);
            }
            if (quotas.unlisted_case_count >= quotas.allowed_unlisted_cases) {
                $('#td_unlisted').html('<font:"color=red">' + $('#td_unlisted').html() + '</font>');
                $('#btn_add').prop('disabled', true);
            }
            if (isNull(quotas.allowed_draft_cases) || quotas.allowed_draft_cases == 0) {
                $('#btn_add').prop('disabled', false);
            }
            if (isNull(quotas.allowed_draft_cases) || quotas.allowed_draft_cases == 0) {
                $('#btn_add').prop('disabled', false);
            }
            if (quotas.unlisted_playlist_count >= quotas.allowed_unlisted_playlists) {
                $('#td_playlists').html('<font:"color=red">' + $('#td_playlists').html() + '</font>');
            }
        }
        function expandedEvents(tr) {                       
            var sb = '<table class="table table-bordered table-condensed">'
            if (!isNull(tr.StudyUid))
                sb += '<tr><td>Study UID:</td><td>' + tr.StudyUid + '</td></tr>';
            if (!isNull(tr.SeriesUid))
                sb += '<tr><td>Series UID:</td><td>' + tr.SeriesUid + '</td></tr>';
            if (!isNull(tr.Data)) {
                sb += '<tr><td colspan="2" style="text-align:center">Data</td></tr>';
                sb += '<tr><td colspan="2">' + tr.Data + '</td></tr>';
            }
            sb += '</table>';
            return sb;
        }
        function chunk(str, n) {
            var ret = [];
            var i;
            var len;

            for (i = 0, len = str.length; i < len; i += n) {
                ret.push(str.substr(i, n))
            }

            return ret
        };
        function showCompleted() {
            ajaxHandler('Handlers/GetTable.ashx?status=Completed&username=' + username, 'get', postGetTable);
            $('#btn_completed').addClass('btn-success');
            $('#btn_pending').removeClass('btn-success');
            $('#btn_errors').removeClass('btn-success');
        }
        function showPending() {
            ajaxHandler('Handlers/GetTable.ashx?status=Pending&username=' + username, 'get', postGetTable);
            $('#btn_completed').removeClass('btn-success');
            $('#btn_pending').addClass('btn-success');
            $('#btn_errors').removeClass('btn-success');
        }
        function showErrors() {
            ajaxHandler('Handlers/GetTable.ashx?status=Error&username=' + username, 'get', postGetTable);
            $('#btn_completed').removeClass('btn-success');
            $('#btn_pending').removeClass('btn-success');
            $('#btn_errors').addClass('btn-success');
        }
        function addnew() {
            window.location.replace('Add.html?username=' + refresh);
        }
        function postGetTable(data) {
            try {
                if (isNull(data)) { return false; }
                if (!data.Success) { notify('warning', data.Message); }
                $('#lbl_tableTitle').html('<b>' + data.Data.Name + '</b>');
                $('#btn_completed').html('<i class="glyphicon glyphicon-check"></i> Completed <span class="badge">' + data.Data.CompletedCount + '</span>');
                $('#btn_pending').html('<i class="glyphicon glyphicon-cloud-upload"></i> Pending <span class="badge">' + data.Data.PendingCount + '</span>');
                $('#btn_errors').html('<i class="glyphicon glyphicon-alert"></i> Errors <span class="badge">' + data.Data.ErrorCount + '</span>');
                var dt = $('#tbl_cases').DataTable();
                dt.clear();
                dt.rows.add(data.Data.Cases).draw();

            } catch (e) { alert(e.message); }
        }
        function showEvents() {
            $('#div_events').show();
            $('#btn_events').hide();
            $('#btn_hide_events').show();
        }
        function hideEvents() {
            $('#div_events').hide();
            $('#btn_events').show();
            $('#btn_hide_events').hide();
        }
        function caseSelect(row) {
            ajaxHandler('Handlers/GetDetailsForStudy.ashx?caseid=' + row.case_id, 'get', populateDetails);
            $('#lbl_events').html('Event Logs for Case: ' + row.case_id);
        }
        function caseDeselect(row) {
            var dt = $('tbl_events').DataTable();
            dt.clear().draw();
            $('#btn_events').show();
            $('#btn_events').prop('disabled', true);
            $('#div_events').hide();
            $('#btn_hide_events').hide();
            $('#lbl_events').html('Event Logs for Case: ');
        }
        function populateDetails(data) {
            try {
                if (isNull(data)) { return false; }
                if (!data.Success) { notify('error', data.Message); return false; }
                var dt = $('#tbl_events').DataTable();
                dt.clear();
                dt.rows.add(data.Data).draw();
                $('#btn_events').prop('disabled', false);
            } catch (e) { alert(e.message); }
        }
        function expandDetails(tr) {
            var sb = "<table>";
            sb += '<tr>'
            sb += '<td valign="top" style="padding-right:20px"><table><tr><td><b>Study Details</b></td></tr>';
            $(tr.study_list).each(function () {
                var study = this;
                var date = (new Date(study.date)).toString("dd/MM/yyyy HH:mm");
                sb += '<tr><td style="padding-left:20px; padding-top:10px"><b>' + date + '</b></td><td style="padding-left:20px; padding-top:10px"><b>[' + study.modality + '] ' + study.description + '</b></td></tr>';
                sb += '<tr><td colspan="3"><table>';
                $(study.series).each(function () {
                    sb += '<tr><td style="padding-left:20px; padding-top:5px">' + this.description + ' (' + this.images + ')</td></tr>';
                })
                sb += '</table></td></tr>';
            });
            if (!hasPatientDetails(tr)) {
                sb += '</table></td>';
                sb += '<td valign="top" style="padding-left:20px; border-left:thin solid #bfbfbf;"><table><tr><td colspan="2"><b>Patient Details</b></td></tr>';
                sb += '<tr><td style="padding-left:20px; padding-top:10px"><b>Patient Name:</b></td><td style="padding-left:10px; padding-top:10px">[Was Not Captured]</td></tr>';
                sb += '<tr><td style="padding-left:20px"><b>Patient ID:</b></td><td style="padding-left:10px">[Was Not Captured]</td></tr>';
                sb += '<tr><td style="padding-left:20px"><b>Patient DOB:</b></td><td style="padding-left:10px">[Was Not Captured]</td></tr>';
                sb += '</table></td></tr></table>';
            } else {
                var dobDate = (new Date(tr.study_list[0].patient_dob)).toString("dd/MM/yyyy");
                sb += '</table></td>';
                sb += '<td valign="top" style="padding-left:20px; border-left:thin solid #bfbfbf;"><table><tr><td colspan="2"><b>Patient Details</b></td></tr>';
                sb += '<tr><td style="padding-left:20px; padding-top:10px"><b>Patient Name:</b></td><td style="padding-left:10px; padding-top:10px">' + tr.study_list[0].patient_name + '</td></tr>';
                sb += '<tr><td style="padding-left:20px"><b>Patient ID:</b></td><td style="padding-left:10px">' + tr.study_list[0].patient_id + '</td></tr>';
                sb += '<tr><td style="padding-left:20px"><b>Patient DOB:</b></td><td style="padding-left:10px">' + dobDate + '</td></tr>';
                sb += '</table></td></tr></table>';
            }                        
            return sb;
        }
        function hasPatientDetails(caseDetails) {
            if (isNull(caseDetails.study_list[0].patient_name) && !caseDetails.study_list[0].patient_name) { return false; }
            if (isNull(caseDetails.study_list[0].patient_id) && !caseDetails.study_list[0].patient_id) { return false; }
            if (isNull(caseDetails.study_list[0].patient_dob) && !caseDetails.study_list[0].patient_dob) { return false; }
            return true;
        }
    </script>

</body>
</html>
