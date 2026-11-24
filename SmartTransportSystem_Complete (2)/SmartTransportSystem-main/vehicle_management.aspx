<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="vehicle_management.aspx.cs"
    Inherits="car_WebApplication1.vehicle_management" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    &nbsp;
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    
    <style>
        .table th, .grid-view th, [id$="gvInsurance"] th, [id$="gvMaintenance"] th, [id$="gvLicense"] th {
            background-color: #a50d25 !important;
            color: white !important;
            font-weight: bold !important;
            padding: 12px !important;
            text-align: center !important;
            border-bottom: 2px solid #a50d25 !important;
        }
        [id$="gvInsurance"] th:hover, [id$="gvMaintenance"] th:hover, [id$="gvLicense"] th:hover {
            background-color: #a50d25 !important;
        }
        .table-striped tbody tr:hover { background-color: rgba(200, 16, 46, 0.05); transition: all 0.3s; }
        .pagination-ys { background: #f8f9fa; padding: 10px; text-align: center; border-radius: 0 0 10px 10px; }
        .pagination-ys a, .pagination-ys span { display: inline-block; padding: 5px 10px; margin: 0 2px; border: 1px solid #dee2e6; border-radius: 4px; text-decoration: none; color: #a50d25; font-weight: bold; transition: all 0.3s; }
        .pagination-ys a:hover { background-color: #a50d25; color: white; border-color: #c8102e; }
        .pagination-ys span { background-color: #a50d25; color: white; border-color: #c8102e; }
        .page-title { font-weight: bold; color: #000; text-align: center; margin-bottom: 30px; }
        .entry-box, .search-box { border: 2px solid #000; border-radius: 14px; padding: 25px; background-color: #fff; margin-bottom: 20px; }
        .result-box { border: 2px solid #000; border-radius: 14px; padding: 25px; margin-top: 20px; background-color: #f9f9f9; }
        .form-label { font-weight: bold; }
        .form-control { text-align: right; }
        .btn-save { background-color: #a50d25; border-color: #c8102e; color: white; font-weight: bold; height: 50px; width: 100%; }
        .btn-save:hover { background-color: #a50d25; }
        .btn-clear, .btn-back { background-color: #6c757d; color: #fff; font-weight: bold; height: 50px; width: 100%; }
        .btn-clear:hover, .btn-back:hover { background-color: #5a6268; }
        .btn-search { background-color: #a50d25; color: #fff; font-weight: bold; height: 50px; width: 100%; }
        .btn-search:hover { background-color: #a50d25; }
        .btn-tab { background-color: transparent; border: none; color: #495057; font-weight: bold; padding: 10px 20px; margin: 0 2px; border-radius: 0; cursor: pointer; }
        .btn-tab.active { color: #a50d25; border-bottom: 3px solid #c8102e; background-color: transparent; }
        .btn-tab:hover { color: #a50d25; }
        .car-number-container { display: flex; align-items: center; gap: 5px; flex-wrap: wrap; background: #f8f9fa; padding: 10px; border-radius: 8px; border: 1px solid #dee2e6; }
        .car-letter-box { width: 55px; }
        .car-number-single-box { width: 100px; }
        .car-letter, .car-number-single { text-align: center; font-size: 18px; font-weight: bold; direction: ltr; }
        .car-letter { background: #fff3f3; border-color: #ce1126; }
        .car-number-single { background: #f0f0f0; border-color: #333; letter-spacing: 2px; }
        .car-number-separator { font-size: 20px; font-weight: bold; color: #ce1126; margin: 0 5px; }
        .car-letter:focus, .car-number-single:focus { outline: none; box-shadow: 0 0 0 3px rgba(206, 17, 38, 0.25); border-color: #ce1126; }
        .car-letter.error, .car-number-single.error { border-color: #dc3545; background-color: #fff8f8; }
        .car-letter.valid, .car-number-single.valid { border-color: #28a745; background-color: #f0fff0; }
        .section-header { background: linear-gradient(90deg, #ce1126, #ffffff); padding: 10px 15px; border-radius: 8px; margin-bottom: 20px; font-weight: bold; color: #000; overflow: auto; }
        .info-card { background: white; border-radius: 10px; padding: 15px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); }
        .required-field::after { content: " *"; color: #ce1126; font-weight: bold; }
        .info-label { font-size: 13px; color: #666; margin-bottom: 5px; }
        .info-value { font-size: 16px; font-weight: bold; color: #333; }
        .warning-box { background: #fff3cd; border-right: 4px solid #ffc107; padding: 12px; margin-top: 15px; border-radius: 8px; color: #856404; font-weight: bold; }
        .km-display-box { background-color: #e3f2fd; text-align: center; padding: 20px; border-radius: 10px; border: 2px solid #0d6efd; box-shadow: 0 2px 8px rgba(13, 110, 253, 0.2); }
        .km-label { font-size: 16px; color: #666; margin-bottom: 5px; }
        .km-value { font-size: 48px; font-weight: bold; line-height: 1.2; }
        .km-unit { font-size: 18px; opacity: 0.8; }
        .km-date { font-size: 14px; color: #666; margin-top: 5px; }
        .km-icon { font-size: 24px; margin-bottom: 10px; }
        .section-buttons { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 20px; }
        .btn-section { background-color: #343a40; color: white; border: none; padding: 10px 20px; border-radius: 8px; font-weight: bold; cursor: pointer; transition: all 0.3s; }
        .btn-section:hover { background-color: #23272b; transform: translateY(-2px); }
        .btn-section.active { background-color: #a50d25; }
        .btn-section i { margin-left: 8px; }
        .section-container { display: none; animation: fadeIn 0.5s; }
        .section-container.active { display: block; }
        @keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        .badge-count { background-color: #a50d25; color: white; border-radius: 50%; padding: 2px 8px; margin-right: 5px; font-size: 12px; }
        .table { width: 100%; margin-bottom: 1rem; background-color: transparent; border-collapse: collapse; }
        .table th { background-color: #343a40; color: white; padding: 12px; font-weight: bold; text-align: center; }
        .table td { padding: 10px; text-align: center; border-bottom: 1px solid #dee2e6; }
        .table-striped tbody tr:nth-of-type(odd) { background-color: rgba(0, 0, 0, .05); }
        .table-bordered { border: 1px solid #dee2e6; }
        .status-active { color: #28a745; font-weight: bold; }
        .status-expired { color: #dc3545; font-weight: bold; }
        .status-warning { color: #ffc107; font-weight: bold; }
        .cost-column { color: #a50d25; font-weight: bold; }
        .km-column { color: #0d6efd; font-weight: bold; }
        .chassis-valid { border-color: #28a745 !important; background-color: #f0fff0 !important; }
        .chassis-invalid { border-color: #dc3545 !important; background-color: #fff8f8 !important; }
        .validation-message { font-size: 12px; margin-top: 5px; }
        .validation-message.error { color: #dc3545; }
        .validation-message.success { color: #28a745; }
        .btn-sm { padding: 5px 10px; font-size: 12px; margin: 0 2px; border-radius: 4px; cursor: pointer; }
        .btn-info { background-color: #17a2b8; border-color: #17a2b8; color: white; border: none; }
        .btn-info:hover { background-color: #138496; }
        .btn-warning { background-color: #ffc107; border-color: #ffc107; color: #000; border: none; }
        .btn-warning:hover { background-color: #e0a800; }
        .float-left { float: left !important; }
        .modal { z-index: 1050; }
        .modal-backdrop { z-index: 1040; }
        .modal-header .close { padding: 1rem; margin: -1rem -1rem -1rem auto; cursor: pointer; color: white; opacity: 1; background: transparent; border: none; font-size: 28px; }
        .modal-header .close:hover { color: white; opacity: 0.8; }
        .document-card { border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 10px; transition: all 0.3s; }
        .document-card:hover { box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
        .document-icon { font-size: 40px; color: #a50d25; }
        .document-info { padding: 10px; }
        .doc-type { font-weight: bold; color: #a50d25; }
        .file-name { font-size: 12px; color: #666; }
        .alert-card {
            border: 2px solid #ffc107;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 20px;
            background-color: #fff3cd;
        }
        .alert-header {
            color: #856404;
            font-weight: bold;
            font-size: 18px;
            margin-bottom: 10px;
        }
        .alert-item {
            background-color: white;
            border-right: 4px solid #dc3545;
            padding: 15px;
            margin-bottom: 15px;
            border-radius: 5px;
            transition: all 0.3s ease;
        }
        .alert-item-warning {
            border-right-color: #ffc107;
            background-color: #fffbf0;
        }
        .alert-item-success {
            border-right-color: #28a745;
            background-color: #f0fff4;
        }
        .badge-danger {
            background-color: #dc3545;
            color: white;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
        }
        .badge-warning {
            background-color: #ffc107;
            color: #212529;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
        }
        .badge-success {
            background-color: #28a745;
            color: white;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
        }
        .alert-message-text {
            background-color: #fff3cd;
            border-radius: 5px;
            padding: 8px 12px;
            color: #856404;
            font-weight: bold;
            margin: 5px 0;
        }
        @media (max-width: 768px) {
            .car-letter-box { width: 45px; }
            .car-number-single-box { width: 80px; }
            .car-number-separator { font-size: 16px; }
            .km-value { font-size: 32px; }
            .section-buttons { flex-direction: column; }
            .status-active {
                color: #28a745;
                font-weight: bold;
                background-color: #d4edda;
                padding: 3px 8px;
                border-radius: 12px;
                display: inline-block;
            }
            .status-warning {
                color: #ffc107;
                font-weight: bold;
                background-color: #fff3cd;
                padding: 3px 8px;
                border-radius: 12px;
                display: inline-block;
            }
            .status-expired {
                color: #dc3545;
                font-weight: bold;
                background-color: #f8d7da;
                padding: 3px 8px;
                border-radius: 12px;
                display: inline-block;
            }
            .modal { z-index: 1050; }
            .modal-backdrop { z-index: 1040; }
            .modal-header { background-color: #a50d25; color: white; border-radius: 10px 10px 0 0; }
            .modal-header .close { color: white; opacity: 1; }
            .modal-header .close:hover { color: white; opacity: 0.8; }
            .detail-row { margin-bottom: 15px; border-bottom: 1px solid #eee; padding-bottom: 10px; }
            .detail-label { font-weight: bold; color: #a50d25; font-size: 14px; margin-bottom: 5px; }
            .detail-value { font-size: 16px; color: #333; }
            .insurance-status-active { color: #28a745; font-weight: bold; background-color: #d4edda; padding: 5px 10px; border-radius: 20px; display: inline-block; }
            .insurance-status-expired { color: #dc3545; font-weight: bold; background-color: #f8d7da; padding: 5px 10px; border-radius: 20px; display: inline-block; }
            .insurance-status-warning { color: #ffc107; font-weight: bold; background-color: #fff3cd; padding: 5px 10px; border-radius: 20px; display: inline-block; }
            .status-active { background-color: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
            .status-expired { background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
            .status-warning { background-color: #fff3cd; color: #856404; border: 1px solid #ffeeba; }
        }
    </style>

    <div class="container mt-4" dir="rtl">
        <div class="text-center mb-4">
            <h2 class="page-title">إدارة المركبات</h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <div class="text-center mb-4" style="border-bottom: 2px solid #dee2e6;">
            <asp:Button ID="btnTabEntry" runat="server" Text="إدخال سيارة جديدة" CssClass="btn-tab active" OnClick="BtnTabEntry_Click" CausesValidation="false" />
            <asp:Button ID="btnTabSearch" runat="server" Text="استعلام عن بيانات سيارة" CssClass="btn-tab" OnClick="BtnTabSearch_Click" CausesValidation="false" />
        </div>

        <asp:MultiView ID="MainMultiView" runat="server" ActiveViewIndex="0">

            <!-- ENTRY VIEW -->
            <asp:View ID="viewEntry" runat="server">
                <div class="entry-box">
                    <div class="section-header">
                        <i class="fas fa-car"></i> رقم السيارة <span class="required-field"></span>
                    </div>
                    <div class="info-card">
                        <div class="row align-items-end">
                            <div class="col-md-12">
                                <label class="form-label required-field">رقم السيارة</label>
                                <div class="car-number-container mb-2">
                                    <div class="car-letter-box">
                                        <asp:TextBox ID="txtCarLetter1" runat="server" CssClass="form-control car-letter" MaxLength="1" placeholder="أ" onkeyup="moveToNext(this, 'txtCarLetter2');" onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, ''); }" onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="car-letter-box">
                                        <asp:TextBox ID="txtCarLetter2" runat="server" CssClass="form-control car-letter" MaxLength="1" placeholder="ب" onkeyup="moveToNext(this, 'txtCarLetter3');" onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter1'); }" onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="car-letter-box">
                                        <asp:TextBox ID="txtCarLetter3" runat="server" CssClass="form-control car-letter" MaxLength="1" placeholder="ج" onkeyup="moveToNext(this, 'txtCarNumbers');" onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter2'); }" onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                                    </div>
                                    <span class="car-number-separator">-</span>
                                    <div class="car-number-single-box">
                                        <asp:TextBox ID="txtCarNumbers" runat="server" CssClass="form-control car-number-single" MaxLength="4" placeholder="1234" onkeypress="return isNumber(event);" autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <small class="text-muted">الصيغة: 1-3 أحرف + 1-4 أرقام</small>
                            </div>
                        </div>
                    </div>

                    <div class="section-header">
                        <i class="fas fa-info-circle"></i> البيانات الأساسية
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-4">
                                <label class="form-label required-field">نوع السيارة (العلامة التجارية)</label>
                                <asp:UpdatePanel ID="upCarType" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlCarType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlCarType_SelectedIndexChanged">
                                            <asp:ListItem Value="">-- اختر نوع السيارة --</asp:ListItem>
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">الموديل</label>
                                <asp:UpdatePanel ID="upCarModel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlCarModel" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="">-- اختر الموديل --</asp:ListItem>
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlCarType" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">هيكل السيارة</label>
                                <asp:UpdatePanel ID="upCarBody" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlCarBody" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlCarBody_SelectedIndexChanged">
                                            <asp:ListItem Value="">-- اختر هيكل السيارة --</asp:ListItem>
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">سنة الصنع</label>
                                <asp:UpdatePanel ID="upYear" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="">-- اختر سنة الصنع --</asp:ListItem>
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">حالة السيارة</label>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر الحالة --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">نوع الوقود</label>
                                <asp:DropDownList ID="ddlFuelType" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر نوع الوقود --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">السياره مزوده ب غاز طبيعي</label>
                                <asp:DropDownList ID="ddlNaturalGas" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر --</asp:ListItem>
                                    <asp:ListItem Value="1">نعم</asp:ListItem>
                                    <asp:ListItem Value="0">لا</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">عدد الركاب</label>
                                <asp:TextBox ID="txtPassengers" runat="server" CssClass="form-control" TextMode="Number" min="1"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">العداد الابتدائي (كم)</label>
                                <asp:TextBox ID="txtInitialKM" runat="server" CssClass="form-control" TextMode="Number" step="0.1" min="0" placeholder="أدخل قراءة العداد الابتدائية"></asp:TextBox>
                                <small class="text-muted">أدخل قراءة العداد عند استلام السيارة</small>
                            </div>
                        </div>
                    </div>

                    <div class="section-header">
                        <i class="fas fa-cogs"></i> البيانات الفنية
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label class="form-label required-field">رقم الشاسيه</label>
                                <asp:TextBox ID="txtChassis" runat="server" CssClass="form-control" MaxLength="17" onkeyup="this.value = this.value.toUpperCase(); validateChassisRealTime();" onblur="validateChassisRealTime();" placeholder="مثال: 1HGCM82633A123456"></asp:TextBox>
                                <div id="chassisValidationMessage" class="validation-message"></div>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label required-field">رقم الموتور</label>
                                <asp:TextBox ID="txtMotor" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col-md-6">
                            <asp:Button ID="btnSave" runat="server" Text="حفظ البيانات" CssClass="btn btn-save" OnClick="BtnSave_Click" OnClientClick="return validateEntryForm();" />
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnClear" runat="server" Text="مسح الحقول" CssClass="btn btn-clear" OnClick="BtnClear_Click" />
                        </div>
                    </div>
                </div>
            </asp:View>

            <!-- SEARCH VIEW -->
            <asp:View ID="viewSearch" runat="server">
                <div class="search-box mb-4">
                    <div class="row align-items-end">

                        <!-- ADMIN: plate number boxes (unchanged). Hidden by the code-behind for a driver login. -->
                        <div id="colPlateSearch" runat="server" class="col-md-8">
                            <label class="form-label">رقم السيارة</label>
                            <div class="car-number-container mb-2">
                                <div class="car-letter-box">
                                    <asp:TextBox ID="txtSearchLetter1" runat="server" CssClass="form-control car-letter" MaxLength="1" placeholder="أ" onkeyup="moveToNextSearch(this, 'txtSearchLetter2');" onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPreviousSearch(this, ''); }" onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="car-letter-box">
                                    <asp:TextBox ID="txtSearchLetter2" runat="server" CssClass="form-control car-letter" MaxLength="1" placeholder="ب" onkeyup="moveToNextSearch(this, 'txtSearchLetter3');" onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPreviousSearch(this, 'txtSearchLetter1'); }" onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="car-letter-box">
                                    <asp:TextBox ID="txtSearchLetter3" runat="server" CssClass="form-control car-letter" MaxLength="1" placeholder="ج" onkeyup="moveToNextSearch(this, 'txtSearchNumbers');" onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPreviousSearch(this, 'txtSearchLetter2'); }" onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                                </div>
                                <span class="car-number-separator">-</span>
                                <div class="car-number-single-box">
                                    <asp:TextBox ID="txtSearchNumbers" runat="server" CssClass="form-control car-number-single" MaxLength="4" placeholder="1234" onkeyup="updateFullCarNumber();" onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPreviousSearch(this, 'txtSearchLetter3'); }" onkeypress="return isNumber(event);" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <asp:HiddenField ID="hdnFullCarNumber" runat="server" />
                        </div>

                        <!-- DRIVER ONLY: dropdown with the car(s) of this driver -->
                        <div id="colDriverCar" runat="server" class="col-md-8" visible="false">
                            <label class="form-label">السيارة</label>
                            <asp:DropDownList ID="ddlDriverCar" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>

                        <div class="col-md-4 text-center">
                            <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-search w-100 mt-3" OnClick="BtnSearch_Click" OnClientClick="return validateSearchForm();" />
                        </div>
                    </div>
                </div>

                <div class="result-box" id="resultBox" runat="server" visible="false">
                    <asp:HiddenField ID="hdnVehicleID" runat="server" Value="" />
                    <asp:HiddenField ID="hdnCarNumberId" runat="server" Value="" />
                    <asp:HiddenField ID="hdnCurrentLicenseId" runat="server" Value="0" />

                    <div class="section-header"><i class="fas fa-car"></i> البيانات الأساسية</div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-4">
                                <div class="info-label">رقم السيارة</div>
                                <div class="info-value"><asp:Label ID="lblCarNumber" runat="server" Font-Bold="true" Font-Size="18px"></asp:Label></div>
                            </div>
                            <div class="col-md-4"><label class="form-label">نوع السيارة (العلامة التجارية)</label><asp:TextBox ID="txtType" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-4"><label class="form-label">الموديل</label><asp:TextBox ID="txtModel" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-4"><label class="form-label">هيكل السيارة</label><asp:TextBox ID="txtCarBody" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-4"><label class="form-label">سنة الصنع</label><asp:TextBox ID="txtYear" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-4"><label class="form-label">حالة السيارة</label><asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                        </div>
                    </div>

                    <div class="section-header"><i class="fas fa-cogs"></i> البيانات الفنية</div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-6"><label class="form-label">رقم الشاسيه</label><asp:TextBox ID="txtChassisView" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-6"><label class="form-label">رقم الموتور</label><asp:TextBox ID="txtMotorView" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-4"><label class="form-label">نوع الوقود</label><asp:TextBox ID="txtGasView" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-4"><label class="form-label">السياره مزوده ب غاز طبيعي</label><asp:TextBox ID="txtNaturalGasView" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-4"><label class="form-label">عدد الركاب</label><asp:TextBox ID="txtPassengersView" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                            <div class="col-md-4"><label class="form-label">نوع الترخيص</label><asp:TextBox ID="txtLicenseTypeView" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                        </div>
                    </div>

                    <div class="section-header"><i class="fas fa-id-card"></i> الترخيص الحالي</div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-3">
                                <label class="form-label">رقم الترخيص</label>
                                <asp:TextBox ID="txtCurrentLicenseNumber" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">بداية الترخيص</label>
                                <asp:TextBox ID="txtLicenseStartView" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">نهاية الترخيص</label>
                                <asp:TextBox ID="txtLicenseEndView" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">وحدة المرور</label>
                                <asp:TextBox ID="txtTrafficUnitView" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="warning-box" id="licenseWarning" runat="server" visible="false">
                            <div id="mandatoryInsuranceContainer" style="display: none; margin-top: 15px;"></div>
                            <i class="fas fa-exclamation-triangle"></i>
                            <asp:Label ID="lblLicenseWarning" runat="server"></asp:Label>
                        </div>
                    </div>

                    <!-- ODOMETER INFORMATION CARDS -->
                    <div class="row g-3 mb-3">
                        <div class="col-md-6">
                            <div class="info-card" style="background-color: #e3f2fd; border-right: 4px solid #0d6efd;">
                                <div class="info-label"><i class="fas fa-flag-checkered"></i> العداد الابتدائي (عند الاستلام)</div>
                                <div class="info-value" style="font-size: 24px; color: #0d6efd;">
                                    <asp:Label ID="lblInitialKM" runat="server" Text="0"></asp:Label> <span style="font-size: 14px;">كم</span>
                                </div>
                                <small>أول قراءة عداد مسجلة للسيارة</small>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="info-card" style="background-color: #d1e7dd; border-right: 4px solid #198754;">
                                <div class="info-label"><i class="fas fa-tachometer-alt"></i> العداد الحالي (آخر قراءة)</div>
                                <div class="info-value" style="font-size: 24px; color: #198754;">
                                    <asp:Label ID="lblCurrentKM" runat="server" Text="0"></asp:Label> <span style="font-size: 14px;">كم</span>
                                </div>
                                <small>آخر قراءة عداد مسجلة من أوامر الشغل</small>
                            </div>
                        </div>
                    </div>

                    <!-- Maintenance Alerts Section -->
                    <asp:Panel ID="pnlAlerts" runat="server" Visible="false">
                        <div class="alert-card">
                            <div class="alert-header">
                                <i class="fas fa-exclamation-triangle"></i>
                                تنبيهات الصيانة
                            </div>

                            <asp:Repeater ID="rptAlerts" runat="server" OnItemDataBound="RptAlerts_ItemDataBound">
                                <ItemTemplate>
                                    <div id="alertItem" runat="server" class="alert-item mb-3">
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <strong><i class="fas fa-wrench"></i>
                                                    <asp:Label ID="lblServiceName" runat="server" Font-Size="16px"></asp:Label>
                                                </strong>
                                                <small class="text-muted me-2">
                                                    (<asp:Label ID="lblMaintenanceType" runat="server"></asp:Label>)
                                                </small>
                                            </div>
                                            <div class="col-md-6 text-md-start">
                                                <span id="badgeSpan" runat="server" class="badge">
                                                    <asp:Label ID="lblAlertMessage" runat="server"></asp:Label>
                                                </span>
                                            </div>
                                        </div>

                                        <div class="row mb-2">
                                            <div class="col-12">
                                                <div class="alert-message-text">
                                                    <i class="fas fa-exclamation-triangle"></i>
                                                    <asp:Label ID="lblAlertMessageText" runat="server" Font-Weight="bold"></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="row">
                                                    <div class="col-6">
                                                        <small class="text-muted">
                                                            <i class="fas fa-road"></i> المسافة:
                                                            <asp:Label ID="lblKMInterval" runat="server" CssClass="fw-bold"></asp:Label>
                                                        </small>
                                                    </div>
                                                    <div class="col-6">
                                                        <small class="text-muted">
                                                            <i class="fas fa-clock"></i> الفترة الزمنية:
                                                            <asp:Label ID="lblTimeInterval" runat="server" CssClass="fw-bold"></asp:Label>
                                                        </small>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="row">
                                                    <div class="col-6">
                                                        <small class="text-muted">
                                                            <i class="fas fa-calendar-alt"></i> آخر صيانة:
                                                            <asp:Label ID="lblLastServiceDate" runat="server" CssClass="fw-bold"></asp:Label>
                                                        </small>
                                                    </div>
                                                    <div class="col-6">
                                                        <small class="text-muted">
                                                            <i class="fas fa-hourglass-half"></i> المتبقي:
                                                            <asp:Label ID="lblRemainingDays" runat="server" CssClass="fw-bold"></asp:Label>
                                                        </small>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row mt-2">
                                            <div class="col-md-6">
                                                <small class="text-muted">
                                                    <i class="fas fa-gauge-high"></i> المتبقي في المسافة:
                                                    <asp:Label ID="lblRemainingKM" runat="server" CssClass="fw-bold"></asp:Label>
                                                </small>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="text-info">
                                                    <i class="fas fa-calendar-alt"></i> الصيانة القادمة:
                                                    <asp:Label ID="lblNextDue" runat="server" CssClass="fw-bold"></asp:Label>
                                                </small>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:Label ID="lblNoAlerts" runat="server"
                                Text="لا توجد تنبيهات للصيانة"
                                CssClass="text-success text-center d-block p-3"
                                Visible="false">
                            </asp:Label>
                        </div>
                    </asp:Panel>

                    <!-- Engine Overhaul Information Section -->
                    <div class="row g-3 mb-3" id="engineOverhaulRow" runat="server" visible="false">
                        <div class="col-md-12">
                            <div class="info-card" style="background: linear-gradient(135deg, #fff8e1 0%, #ffe0b2 100%); border-right: 4px solid #ff9800;">
                                <div class="info-label"><i class="fas fa-oil-can"></i> عمرة الموتور</div>
                                <div class="row mt-2">
                                    <div class="col-md-3">
                                        <div class="info-label" style="font-size: 12px;">تاريخ العمرة</div>
                                        <div class="info-value" style="font-size: 16px; font-weight: bold; color: #ff9800;">
                                            <asp:Label ID="lblEngineOverhaulDate" runat="server" Text="—"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="info-label" style="font-size: 12px;">قراءة العداد عند العمرة</div>
                                        <div class="info-value" style="font-size: 16px; font-weight: bold; color: #ff9800;">
                                            <asp:Label ID="lblEngineOverhaulOdometer" runat="server" Text="—"></asp:Label> <span style="font-size: 12px;">كم</span>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="info-label" style="font-size: 12px;">المسافة منذ العمرة</div>
                                        <div class="info-value" style="font-size: 16px; font-weight: bold; color: #ff9800;">
                                            <asp:Label ID="lblEngineOverhaulDistance" runat="server" Text="—"></asp:Label> <span style="font-size: 12px;">كم</span>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="info-label" style="font-size: 12px;">تكلفة العمرة</div>
                                        <div class="info-value" style="font-size: 16px; font-weight: bold; color: #ff9800;">
                                            <asp:Label ID="lblEngineOverhaulCost" runat="server" Text="—"></asp:Label> <span style="font-size: 12px;">ج.م</span>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-12">
                                        <div class="info-label" style="font-size: 12px;">ملاحظات العمرة</div>
                                        <div class="info-value" style="font-size: 14px;">
                                            <asp:Label ID="lblEngineOverhaulNotes" runat="server" Text="—"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-12">
                                        <div id="overhaulStatusDiv" runat="server" class="alert" style="padding: 8px; margin-top: 10px; font-size: 13px;">
                                            <asp:Label ID="lblOverhaulStatus" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="section-header mt-3"><i class="fas fa-sticky-note"></i> الملاحظات</div>
                    <div class="info-card">
                        <asp:TextBox ID="txtNoteView" runat="server" CssClass="form-control" ReadOnly="true" TextMode="MultiLine" Rows="3" />
                    </div>

                    <div class="section-buttons mt-4">
                        <button type="button" class="btn-section" onclick="showSection('insurance')"><i class="fas fa-shield-alt"></i> التأمين <span class="badge-count" id="insuranceCount">0</span></button>
                        <button type="button" class="btn-section" onclick="showSection('maintenance')"><i class="fas fa-wrench"></i> الصيانة <span class="badge-count" id="maintenanceCount">0</span></button>
                        <button type="button" class="btn-section" onclick="showSection('license')"><i class="fas fa-id-card"></i> سجل التراخيص <span class="badge-count" id="licenseCount">0</span></button>
                    </div>

                    <div id="insuranceSection" class="section-container">
                        <div class="section-header"><i class="fas fa-shield-alt"></i> سجل التأمين</div>
                        <div class="info-card">
                            <asp:UpdatePanel ID="upInsurance" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvInsurance" runat="server" AutoGenerateColumns="False" 
                                        AllowPaging="True" PageSize="10" OnPageIndexChanging="GvInsurance_PageIndexChanging"
                                        CssClass="table table-bordered table-hover" GridLines="None" Width="100%">
                                        <Columns>
                                            <asp:BoundField DataField="company_name" HeaderText="شركة التأمين" />
                                            <asp:BoundField DataField="insurance_name" HeaderText="نوع التأمين" />
                                            <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="cost" HeaderText="التكلفة" DataFormatString="{0:N2}" />
                                            <asp:TemplateField HeaderText="عرض">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnViewInsurance" runat="server" 
                                                        CssClass="btn btn-sm btn-info" 
                                                        OnClick="ViewInsurance_Click"
                                                        CommandArgument='<%# Eval("insurance_id") %>'
                                                        ToolTip="عرض التفاصيل">
                                                        <i class="fas fa-eye"></i> عرض
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <div id="maintenanceSection" class="section-container">
                        <div class="section-header"><i class="fas fa-wrench"></i> سجل الصيانة</div>
                        <div class="info-card">
                            <asp:UpdatePanel ID="upMaintenance" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvMaintenance" runat="server" AutoGenerateColumns="False" 
                                        AllowPaging="True" PageSize="10" OnPageIndexChanging="GvMaintenance_PageIndexChanging"
                                        CssClass="table table-bordered table-hover" GridLines="None" Width="100%"
                                        EmptyDataText="لا توجد بيانات صيانة مسجلة لهذه السيارة">
                                        <Columns>
                                            <asp:BoundField DataField="MaintenanceDate" HeaderText="تاريخ الصيانة" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="MaintenanceType" HeaderText="نوع الصيانة" />
                                            <asp:BoundField DataField="OdometerReading" HeaderText="قراءة العداد" DataFormatString="{0:N0}" ItemStyle-CssClass="km-column" />
                                            <asp:BoundField DataField="TotalCost" HeaderText="التكلفة" DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
                                            <asp:BoundField DataField="PartsNames" HeaderText="قطع الغيار" />
                                            <asp:TemplateField HeaderText="عرض">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnViewMaintenance" runat="server" 
                                                        CssClass="btn btn-sm btn-info" 
                                                        OnClick="ViewMaintenanceDetails_Click"
                                                        CommandArgument='<%# Eval("MaintenanceID") %>'
                                                        ToolTip="عرض التفاصيل">
                                                        <i class="fas fa-eye"></i> عرض
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <div id="licenseSection" class="section-container">
                        <div class="section-header"><i class="fas fa-id-card"></i> سجل التراخيص السابقة</div>
                        <div class="info-card">
                            <asp:UpdatePanel ID="upLicense" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvLicense" runat="server" 
                                        CssClass="grid-view table table-bordered table-hover" 
                                        AutoGenerateColumns="False" 
                                        EmptyDataText="لا توجد تراخيص سابقة"
                                        ShowHeaderWhenEmpty="True"
                                        AllowPaging="True"
                                        PageSize="5"
                                        OnPageIndexChanging="GvLicense_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="license_number" HeaderText="رقم الترخيص" />
                                            <asp:BoundField DataField="license_type" HeaderText="نوع الترخيص" />
                                            <asp:BoundField DataField="issue_date" HeaderText="تاريخ الإصدار" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="expiry_date" HeaderText="تاريخ الانتهاء" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="traffic_unit" HeaderText="وحدة المرور" />
                                            <asp:TemplateField HeaderText="الحالة">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatus" runat="server" 
                                                        CssClass='<%# Convert.ToDateTime(Eval("expiry_date")) > DateTime.Now ? "status-active" : "status-expired" %>'
                                                        Text='<%# Convert.ToDateTime(Eval("expiry_date")) > DateTime.Now ? "ساري" : "منتهي" %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="عرض">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnViewLicense" runat="server" 
                                                        CssClass="btn btn-sm btn-info" 
                                                        OnClick="ViewLicenseDetails_Click"
                                                        CommandArgument='<%# Eval("license_id") %>'
                                                        ToolTip="عرض التفاصيل">
                                                        <i class="fas fa-eye"></i> عرض
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div> <!-- Close result-box -->
            </asp:View>
        </asp:MultiView>
    </div>

    <!-- Modal for details -->
    <div id="detailsModal" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(0,0,0,0.5); z-index: 9999; direction: rtl;">
        <div style="position: relative; width: 85%; max-width: 900px; margin: 50px auto; background: white; border-radius: 10px; box-shadow: 0 5px 20px rgba(0,0,0,0.3);">
            <div style="background-color: #a50d25; color: white; padding: 15px 20px; border-radius: 10px 10px 0 0; display: flex; justify-content: space-between; align-items: center;">
                <h5 style="margin: 0;"><i class="fas fa-info-circle"></i> <span id="modalTitle">تفاصيل</span></h5>
                <button type="button" id="modalCloseBtn" style="background: transparent; border: none; color: white; font-size: 28px; cursor: pointer; padding: 0; line-height: 1;">&times;</button>
            </div>
            <div style="padding: 20px; max-height: 500px; overflow-y: auto;">
                <div id="detailsContainer" style="margin: 0;"></div>
            </div>
            <div style="padding: 15px 20px; border-top: 1px solid #dee2e6; text-align: left;">
                <button type="button" id="modalFooterCloseBtn" style="background-color: #6c757d; color: white; border: none; padding: 8px 20px; border-radius: 5px; cursor: pointer;">إغلاق</button>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function moveToNext(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) nextField.focus();
            }
            current.classList.remove('error');
        }

        function moveToPrevious(current, prevFieldId) {
            if (current.value.length === 0 && prevFieldId) {
                var prevField = document.getElementById(prevFieldId);
                if (prevField) prevField.focus();
            }
        }

        function moveToNextSearch(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) nextField.focus();
            }
        }

        function moveToPreviousSearch(current, prevFieldId) {
            if (current.value.length === 0 && prevFieldId) {
                var prevField = document.getElementById(prevFieldId);
                if (prevField) prevField.focus();
            }
        }

        function isArabicLetter(event) {
            var charCode = event.which || event.keyCode;
            var charStr = String.fromCharCode(charCode);
            var arabicPattern = /[\u0600-\u06FF]/;
            if (charCode === 8 || charCode === 9 || charCode === 13 || charCode === 37 || charCode === 39 || charCode === 46) return true;
            return arabicPattern.test(charStr);
        }

        function isNumber(event) {
            var charCode = event.which || event.keyCode;
            if (charCode >= 48 && charCode <= 57) return true;
            if (charCode === 8 || charCode === 9 || charCode === 13 || charCode === 37 || charCode === 39 || charCode === 46) return true;
            var arabicNumberPattern = /[\u0660-\u0669]/;
            return arabicNumberPattern.test(String.fromCharCode(charCode));
        }

        function validateChassisNumber(chassisNumber) {
            if (!chassisNumber || chassisNumber.trim() === '') return { valid: false, message: 'رقم الشاسيه مطلوب' };
            chassisNumber = chassisNumber.trim().toUpperCase();
            if (chassisNumber.length !== 17) return { valid: false, message: 'رقم الشاسيه يجب أن يكون 17 رمزاً بالضبط' };
            if (chassisNumber.indexOf('I') !== -1 || chassisNumber.indexOf('O') !== -1) return { valid: false, message: 'رقم الشاسيه لا يمكن أن يحتوي على الحرفين I أو O' };
            var allowedPattern = /^[A-HJ-NPR-Z0-9]{17}$/;
            if (!allowedPattern.test(chassisNumber)) return { valid: false, message: 'رقم الشاسيه يجب أن يحتوي فقط على أرقام (0-9) وحروف أبجدية (ما عدا I, O)' };
            return { valid: true, message: '✅ رقم الشاسيه صحيح' };
        }

        function validateChassisRealTime() {
            var chassisInput = document.getElementById('<%= txtChassis.ClientID %>');
            var messageDiv = document.getElementById('chassisValidationMessage');
            if (!chassisInput || !messageDiv) return;
            if (chassisInput.value) {
                var result = validateChassisNumber(chassisInput.value);
                if (!result.valid) {
                    chassisInput.classList.remove('chassis-valid');
                    chassisInput.classList.add('chassis-invalid');
                    messageDiv.className = 'validation-message error';
                    messageDiv.innerHTML = '❌ ' + result.message;
                } else {
                    chassisInput.classList.remove('chassis-invalid');
                    chassisInput.classList.add('chassis-valid');
                    messageDiv.className = 'validation-message success';
                    messageDiv.innerHTML = '✅ ' + result.message;
                }
            } else {
                chassisInput.classList.remove('chassis-valid', 'chassis-invalid');
                messageDiv.className = 'validation-message';
                messageDiv.innerHTML = '';
            }
        }

        function validateEntryForm() {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');
            if (!letter1.value || !numbers.value) {
                alert('❌ الرجاء إدخال رقم السيارة (حرف واحد على الأقل والأرقام)');
                return false;
            }
            var ddlCarType = document.getElementById('<%= ddlCarType.ClientID %>');
            if (ddlCarType && ddlCarType.value === '') { alert('❌ الرجاء اختيار نوع السيارة (العلامة التجارية)'); ddlCarType.focus(); return false; }
            var ddlCarModel = document.getElementById('<%= ddlCarModel.ClientID %>');
            if (ddlCarModel && ddlCarModel.value === '') { alert('❌ الرجاء اختيار الموديل'); ddlCarModel.focus(); return false; }
            var ddlCarBody = document.getElementById('<%= ddlCarBody.ClientID %>');
            if (ddlCarBody && ddlCarBody.value === '') { alert('❌ الرجاء اختيار هيكل السيارة'); ddlCarBody.focus(); return false; }
            var ddlYear = document.getElementById('<%= ddlYear.ClientID %>');
            if (ddlYear && ddlYear.value === '') { alert('❌ الرجاء اختيار سنة الصنع'); ddlYear.focus(); return false; }
            var chassisInput = document.getElementById('<%= txtChassis.ClientID %>');
            if (chassisInput && chassisInput.value) {
                var chassisResult = validateChassisNumber(chassisInput.value);
                if (!chassisResult.valid) { alert('❌ ' + chassisResult.message); chassisInput.focus(); return false; }
            } else { alert('❌ الرجاء إدخال رقم الشاسيه'); chassisInput.focus(); return false; }
            return true;
        }

        function validateSearchForm() {
            // DRIVER ONLY: the car is chosen from the dropdown (this element exists only for a driver login)
            var ddlDriverCar = document.getElementById('<%= ddlDriverCar.ClientID %>');
            if (ddlDriverCar) {
                if (!ddlDriverCar.value) {
                    alert('❌ الرجاء اختيار السيارة');
                    ddlDriverCar.focus();
                    return false;
                }
                return true;
            }

            var letter1 = document.getElementById('<%= txtSearchLetter1.ClientID %>');
            var numbers = document.getElementById('<%= txtSearchNumbers.ClientID %>');
            if (!letter1.value || !numbers.value) {
                alert('❌ الرجاء إدخال رقم السيارة (حرف واحد على الأقل والأرقام)');
                return false;
            }
            updateFullCarNumber();
            return true;
        }

        function updateFullCarNumber() {
            var letter1 = document.getElementById('<%= txtSearchLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtSearchLetter2.ClientID %>');
            var letter3 = document.getElementById('<%= txtSearchLetter3.ClientID %>');
            var numbers = document.getElementById('<%= txtSearchNumbers.ClientID %>');
            var hiddenField = document.getElementById('<%= hdnFullCarNumber.ClientID %>');
            if (letter1 && numbers && hiddenField) {
                var fullNumber = letter1.value;
                if (letter2 && letter2.value) fullNumber += ' ' + letter2.value;
                if (letter3 && letter3.value) fullNumber += ' ' + letter3.value;
                fullNumber += ' ' + numbers.value;
                hiddenField.value = fullNumber;
            }
        }

        function showSection(sectionName) {
            var sections = document.getElementsByClassName('section-container');
            for (var i = 0; i < sections.length; i++) sections[i].classList.remove('active');
            var buttons = document.getElementsByClassName('btn-section');
            for (var i = 0; i < buttons.length; i++) buttons[i].classList.remove('active');
            var selectedSection = document.getElementById(sectionName + 'Section');
            if (selectedSection) selectedSection.classList.add('active');
            if (event && event.target) event.target.classList.add('active');
        }

        var modalVisible = false;

        function showDetailsModal(title, data) {
            try {
                var modalTitleSpan = document.getElementById('modalTitle');
                if (modalTitleSpan) modalTitleSpan.innerHTML = title;
                var container = document.getElementById('detailsContainer');
                if (!container) return;
                var html = '<div class="row">';
                for (var key in data) {
                    if (data.hasOwnProperty(key)) {
                        html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">' + key + ':</strong><br><span class="text-dark">' + (data[key] || '—') + '</span></div></div>';
                    }
                }
                html += '</div>';
                container.innerHTML = html || '<div class="col-12 text-center">لا توجد تفاصيل لعرضها</div>';
                var modal = document.getElementById('detailsModal');
                if (modal) {
                    modal.style.display = 'block';
                    modalVisible = true;
                }
            } catch (e) { console.error('Error showing modal:', e); alert('حدث خطأ في عرض البيانات: ' + e.message); }
        }

        function closeDetailsModal() {
            var modal = document.getElementById('detailsModal');
            if (modal) {
                modal.style.display = 'none';
                modalVisible = false;
            }
        }

        function showLicenseDetailsWithInsurance(data) {
            try {
                var modalTitleSpan = document.getElementById('modalTitle');
                if (modalTitleSpan) modalTitleSpan.innerHTML = 'تفاصيل الترخيص';
                var container = document.getElementById('detailsContainer');
                if (!container) return;
                var html = '<div class="row">';
                html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">رقم الترخيص:</strong><br><span class="text-dark">' + (data.license_number || '-') + '</span></div></div>';
                html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">نوع الترخيص:</strong><br><span class="text-dark">' + (data.license_type || '-') + '</span></div></div>';
                html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">تاريخ الإصدار:</strong><br><span class="text-dark">' + (data.issue_date || '-') + '</span></div></div>';
                html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">تاريخ الانتهاء:</strong><br><span class="text-dark">' + (data.expiry_date || '-') + '</span></div></div>';
                html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">وحدة المرور:</strong><br><span class="text-dark">' + (data.traffic_unit || '-') + '</span></div></div>';
                html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">تكلفة التجديد:</strong><br><span class="text-dark">' + (data.renewal_cost || '0') + ' ج.م</span></div></div>';
                html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">رقم التسجيل:</strong><br><span class="text-dark">' + (data.registration_number || '-') + '</span></div></div>';
                if (data.mandatory_insurance_company && data.mandatory_insurance_company !== '-') {
                    html += '<div class="col-md-12 mt-3"><hr /><h5 class="text-primary"><i class="fas fa-shield-alt"></i> التأمين الإجباري المرتبط</h5></div>';
                    html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">شركة التأمين:</strong><br><span class="text-dark">' + data.mandatory_insurance_company + '</span></div></div>';
                    html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">تكلفة التأمين:</strong><br><span class="text-dark">' + data.mandatory_insurance_cost + ' ج.م</span></div></div>';
                    if (data.mandatory_insurance_start_date && data.mandatory_insurance_start_date !== '-') {
                        html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">بداية التأمين:</strong><br><span class="text-dark">' + data.mandatory_insurance_start_date + '</span></div></div>';
                        html += '<div class="col-md-6 mb-3"><div class="border-bottom pb-1"><strong class="text-primary">نهاية التأمين:</strong><br><span class="text-dark">' + data.mandatory_insurance_end_date + '</span></div></div>';
                    }
                }
                html += '</div>';
                container.innerHTML = html;
                var modal = document.getElementById('detailsModal');
                if (modal) {
                    modal.style.display = 'block';
                    modalVisible = true;
                }
            } catch (e) { console.error('Error showing license details:', e); alert('حدث خطأ في عرض البيانات: ' + e.message); }
        }

        document.addEventListener('DOMContentLoaded', function () {
            var numbersField = document.getElementById('<%= txtSearchNumbers.ClientID %>');
            if (numbersField) numbersField.addEventListener('blur', updateFullCarNumber);
            validateChassisRealTime();
            var closeBtn = document.getElementById('modalCloseBtn');
            if (closeBtn) closeBtn.onclick = closeDetailsModal;
            var footerCloseBtn = document.getElementById('modalFooterCloseBtn');
            if (footerCloseBtn) footerCloseBtn.onclick = closeDetailsModal;
            var modal = document.getElementById('detailsModal');
            if (modal) modal.onclick = function (e) { if (e.target === modal) closeDetailsModal(); };
            document.onkeyup = function (e) { if (e.keyCode === 27 && modalVisible) closeDetailsModal(); };
        });
    </script>
</asp:Content>