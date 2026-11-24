<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="vehicle_full_management.aspx.cs"
    Inherits="car_WebApplication1.VehicleFullManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    /* ===== General Styles ===== */
    .page-title {
        font-weight: bold;
        color: #000;
        text-align: center;
        margin-bottom: 30px;
        font-size: 32px;
    }

    .main-box {
        border: 2px solid #000;
        border-radius: 14px;
        padding: 30px;
        background-color: #fff;
        margin-bottom: 20px;
    }

    .section-header {
        background: linear-gradient(90deg, #ce1126, #ffffff);
        padding: 12px 18px;
        border-radius: 8px;
        margin-bottom: 20px;
        font-weight: bold;
        color: #000;
        overflow: auto;
        font-size: 18px;
    }

    .form-label {
        font-weight: bold;
        font-size: 15px;
        margin-bottom: 6px;
    }

    .required-field::after {
        content: " *";
        color: #ce1126;
        font-weight: bold;
    }

    /* ===== Buttons ===== */
    .btn-save {
        background-color: #a50d25;
        border-color: #c8102e;
        color: white;
        font-weight: bold;
        height: 52px;
        width: 100%;
        font-size: 17px;
    }
    .btn-save:hover { background-color: #a50d25; }

    .btn-search {
        background-color: #a50d25;
        border-color: #c8102e;
        color: white;
        font-weight: bold;
        height: 52px;
        width: 100%;
        font-size: 17px;
    }
    .btn-search:hover { background-color: #a50d25; }

    .btn-back {
        background-color: #6c757d;
        color: #fff;
        font-weight: bold;
        height: 52px;
        width: 100%;
        font-size: 17px;
    }
    .btn-back:hover { background-color: #5a6268; }

    .btn-clear {
        background-color: #6c757d;
        color: #fff;
        font-weight: bold;
        height: 52px;
        width: 100%;
        font-size: 17px;
    }
    .btn-clear:hover { background-color: #5a6268; }

    .btn-renew {
        background-color: #28a745;
        border-color: #28a745;
        color: white;
        font-weight: bold;
        height: 42px;
        padding: 0 25px;
        font-size: 15px;
    }
    .btn-renew:hover { background-color: #218838; }

    .btn-add {
        background-color: #17a2b8;
        border-color: #17a2b8;
        color: white;
        font-weight: bold;
        height: 42px;
        padding: 0 25px;
        font-size: 15px;
    }
    .btn-add:hover { background-color: #138496; }

    .btn-sm {
        padding: 6px 12px;
        font-size: 13px;
        margin: 0 2px;
        border-radius: 4px;
        cursor: pointer;
        border: none;
    }
    .btn-info { background-color: #17a2b8; color: white; }
    .btn-info:hover { background-color: #138496; }
    .btn-danger { background-color: #dc3545; color: white; }
    .btn-danger:hover { background-color: #c82333; }
    .btn-warning { background-color: #ffc107; color: #212529; }
    .btn-warning:hover { background-color: #e0a800; }

    .btn-overhaul {
        background-color: #ff9800;
        border-color: #ff9800;
        color: white;
        font-weight: bold;
        height: 48px;
        font-size: 15px;
    }
    .btn-overhaul:hover { background-color: #e68900; color: white; }

    .btn-clear-overhaul {
        background-color: #6c757d;
        border-color: #6c757d;
        color: white;
        font-weight: bold;
        height: 48px;
        font-size: 15px;
    }
    .btn-clear-overhaul:hover { background-color: #5a6268; color: white; }

    .btn-add-part {
        background-color: #28a745;
        border-color: #28a745;
        color: white;
        font-weight: bold;
        margin-top: 10px;
        height: 40px;
        font-size: 14px;
        padding: 0 20px;
    }
    .btn-add-part:hover { background-color: #218838; }

    /* ===== Car Number Input ===== */
    .car-number-container {
        display: flex;
        align-items: center;
        gap: 5px;
        flex-wrap: wrap;
        background: #f8f9fa;
        padding: 10px;
        border-radius: 8px;
        border: 1px solid #dee2e6;
    }

    .car-letter-box { width: 65px; }
    .car-number-single-box { width: 110px; }

    .car-letter, .car-number-single {
        text-align: center;
        font-size: 20px;
        font-weight: bold;
        direction: ltr;
        height: 45px;
    }

    .car-letter {
        background: #fff3f3;
        border-color: #ce1126;
    }

    .car-number-single {
        background: #f0f0f0;
        border-color: #333;
        letter-spacing: 2px;
    }

    .car-number-separator {
        font-size: 22px;
        font-weight: bold;
        color: #ce1126;
        margin: 0 5px;
    }

    /* ===== Car Info Card ===== */
    .car-info-card {
        background: #808080;
        color: white;
        border-radius: 10px;
        padding: 18px;
        margin-bottom: 20px;
    }

    .car-number-display {
        font-size: 32px;
        font-weight: bold;
        text-align: center;
        letter-spacing: 2px;
    }

    /* ===== Current License Card ===== */
    .current-license-card {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border-radius: 10px;
        padding: 25px;
        margin-bottom: 20px;
        box-shadow: 0 4px 15px rgba(0,0,0,0.1);
    }

    .current-license-label {
        font-size: 14px;
        opacity: 0.9;
        margin-bottom: 5px;
    }

    .current-license-value {
        font-size: 20px;
        font-weight: bold;
    }

    /* ===== Tabs ===== */
    .nav-tabs-custom {
        border-bottom: 2px solid #dee2e6;
        margin-bottom: 20px;
        display: flex;
        flex-wrap: wrap;
        gap: 2px;
    }

    .nav-tabs-custom .nav-item {
        margin-bottom: -2px;
    }

    .nav-tabs-custom .nav-link {
        background: transparent;
        border: none;
        color: #495057;
        font-weight: bold;
        padding: 12px 24px;
        border-radius: 0;
        cursor: pointer;
        transition: all 0.3s;
        position: relative;
        font-size: 16px;
    }

    .nav-tabs-custom .nav-link:hover {
        color: #a50d25;
        background: #f8f9fa;
    }

    .nav-tabs-custom .nav-link.active {
        color: #a50d25;
        border-bottom: 3px solid #c8102e;
        background: transparent;
    }

    .nav-tabs-custom .nav-link .badge-count {
        background-color: #a50d25;
        color: white;
        border-radius: 50%;
        padding: 2px 8px;
        margin-right: 5px;
        font-size: 12px;
    }

    .tab-content {
        padding: 15px 0;
    }

    .tab-pane {
        display: none;
        animation: fadeIn 0.4s;
    }

    .tab-pane.active {
        display: block;
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }

    /* ===== Grid Views ===== */
    .grid-view {
        width: 100%;
        border-collapse: collapse;
        background: white;
        border-radius: 10px;
        overflow: hidden;
        box-shadow: 0 0 20px rgba(0,0,0,0.1);
    }

    .grid-view th {
        background-color: #a50d25;
        color: white;
        font-weight: bold;
        padding: 14px;
        text-align: center;
        font-size: 14px;
    }

    .grid-view td {
        padding: 12px;
        border-bottom: 1px solid #dee2e6;
        text-align: center;
        font-size: 14px;
    }

    .grid-view tr:hover {
        background-color: #f5f5f5;
    }

    .grid-view .cost-column {
        color: #a50d25;
        font-weight: bold;
    }

    .grid-view .km-column {
        color: #28a745;
        font-weight: bold;
    }

    .pagination-ys {
        background: #f8f9fa;
        padding: 10px;
        text-align: center;
        border-radius: 0 0 10px 10px;
    }

    .pagination-ys a, .pagination-ys span {
        display: inline-block;
        padding: 5px 10px;
        margin: 0 2px;
        border: 1px solid #dee2e6;
        border-radius: 4px;
        text-decoration: none;
        color: #a50d25;
        font-weight: bold;
    }

    .pagination-ys a:hover {
        background-color: #a50d25;
        color: white;
    }

    .pagination-ys span {
        background-color: #a50d25;
        color: white;
    }

    /* ===== Status Badges ===== */
    .status-active {
        color: #28a745;
        font-weight: bold;
        background: #d4edda;
        padding: 4px 12px;
        border-radius: 12px;
        display: inline-block;
        font-size: 13px;
    }

    .status-expired {
        color: #dc3545;
        font-weight: bold;
        background: #f8d7da;
        padding: 4px 12px;
        border-radius: 12px;
        display: inline-block;
        font-size: 13px;
    }

    .status-warning {
        color: #ffc107;
        font-weight: bold;
        background: #fff3cd;
        padding: 4px 12px;
        border-radius: 12px;
        display: inline-block;
        font-size: 13px;
    }

    /* ===== Engine Overhaul Card ===== */
    .engine-overhaul-card {
        background: linear-gradient(135deg, #fff8e1 0%, #ffe0b2 100%);
        border: 2px solid #ff9800 !important;
        border-radius: 10px;
        padding: 25px;
        margin-bottom: 20px;
    }

    .overhaul-status {
        padding: 12px;
        border-radius: 8px;
        font-weight: bold;
        text-align: center;
        font-size: 17px;
    }

    .overhaul-warning {
        background-color: #ffebee;
        border: 1px solid #f44336;
    }

    .overhaul-good {
        background-color: #e8f5e9;
        border: 1px solid #4caf50;
    }

    .overhaul-warning-text {
        color: #f44336;
        font-weight: bold;
    }

    .overhaul-info-text {
        color: #ff9800;
        font-weight: bold;
    }

    .overhaul-success-text {
        color: #4caf50;
        font-weight: bold;
    }

    /* ===== Insurance Info Card ===== */
    .insurance-info-card {
        background: #e8f4f8;
        border-right: 4px solid #17a2b8;
        padding: 18px;
        margin-top: 15px;
        border-radius: 8px;
    }

    .insurance-label {
        font-size: 13px;
        color: #666;
        margin-bottom: 3px;
    }

    .insurance-value {
        font-size: 15px;
        font-weight: bold;
        color: #333;
    }

    /* ===== Current Odometer ===== */
    .current-odometer {
        background-color: #e8f5e9;
        border: 2px solid #28a745;
        border-radius: 10px;
        padding: 18px;
        margin-bottom: 20px;
        text-align: center;
    }

    .current-odometer-label {
        font-size: 18px;
        color: #28a745;
        font-weight: bold;
    }

    .current-odometer-value {
        font-size: 32px;
        font-weight: bold;
        color: #28a745;
    }

    /* ===== Statistics Cards ===== */
    .statistics-card {
        background-color: #f8f9fa;
        border-radius: 10px;
        padding: 18px;
        text-align: center;
        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        margin-bottom: 15px;
    }

    .statistics-value {
        font-size: 28px;
        font-weight: bold;
        color: #c8102e;
    }

    .statistics-card h5 {
        font-size: 17px;
        margin-bottom: 10px;
    }

    /* ===== Alert/Warning Boxes ===== */
    .alert-warning-custom {
        background-color: #fff3cd;
        border-color: #ffc107;
        color: #856404;
        padding: 15px;
        border-radius: 8px;
        margin-bottom: 20px;
        text-align: center;
        font-size: 16px;
    }

    /* ===== Parts Container ===== */
    .parts-container {
        border: 1px solid #dee2e6;
        border-radius: 8px;
        padding: 18px;
        margin-top: 10px;
        background-color: #f8f9fa;
    }

    .part-row {
        margin-bottom: 10px;
        display: flex;
        gap: 10px;
        align-items: center;
    }

    .part-row select {
        flex: 1;
        font-size: 15px;
        padding: 10px;
        height: 45px;
    }

    .part-row button {
        width: 45px;
        height: 45px;
        background-color: #dc3545;
        color: white;
        border: none;
        border-radius: 5px;
        cursor: pointer;
        padding: 6px;
        font-size: 16px;
    }

    .part-row button:hover {
        background-color: #c82333;
    }

    /* ===== Modal ===== */
    .modal-custom {
        display: none;
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.6);
        z-index: 10000;
        direction: rtl;
    }

    .modal-custom .modal-content {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        width: 85%;
        max-width: 850px;
        background: white;
        border-radius: 12px;
        box-shadow: 0 5px 25px rgba(0,0,0,0.3);
    }

    .modal-custom .modal-header {
        background: #a50d25;
        color: white;
        padding: 18px 25px;
        border-radius: 12px 12px 0 0;
        display: flex;
        justify-content: space-between;
        align-items: center;
        font-size: 18px;
    }

    .modal-custom .modal-header .close {
        background: none;
        border: none;
        color: white;
        font-size: 32px;
        cursor: pointer;
    }

    .modal-custom .modal-body {
        padding: 25px;
        max-height: 450px;
        overflow-y: auto;
        font-size: 16px;
    }

    .modal-custom .modal-footer {
        padding: 15px 25px;
        border-top: 1px solid #ddd;
        text-align: left;
    }

    .modal-custom .modal-footer .btn-secondary {
        background: #6c757d;
        color: white;
        border: none;
        padding: 10px 25px;
        border-radius: 5px;
        cursor: pointer;
        font-size: 15px;
    }

    /* ===== No License Message ===== */
    .no-license-message {
        background: #f8f9fa;
        border: 2px dashed #dee2e6;
        border-radius: 10px;
        padding: 35px;
        text-align: center;
        color: #6c757d;
        margin-bottom: 20px;
    }

    .no-license-message i {
        font-size: 45px;
        color: #ce1126;
        margin-bottom: 10px;
        display: block;
    }

    .no-license-message h5 {
        font-size: 20px;
        margin-bottom: 5px;
    }

    .no-license-message p {
        font-size: 15px;
        margin-bottom: 0;
    }

    /* ===== Form Controls ===== */
    .form-control {
        font-size: 15px;
        padding: 10px 14px;
        height: 42px;
    }

    .form-control[type="date"] {
        height: 42px;
    }

    textarea.form-control {
        height: auto;
        min-height: 70px;
    }

    select.form-control {
        height: 42px;
        padding: 8px 12px;
    }

    /* ===== Alerts ===== */
    .alert {
        font-size: 16px;
        padding: 15px 20px;
    }

    /* ===== Utility Classes ===== */
    .float-left { 
        float: left !important; 
    }

    /* ===== Responsive ===== */
    @media (max-width: 768px) {
        .page-title {
            font-size: 24px;
        }
        
        .main-box {
            padding: 15px !important;
        }
        
        .car-letter-box { width: 48px !important; }
        .car-number-single-box { width: 80px !important; }
        
        .car-letter, .car-number-single {
            font-size: 17px !important;
            height: 38px !important;
            padding: 6px !important;
        }
        
        .car-number-display {
            font-size: 22px !important;
        }
        
        .current-license-value {
            font-size: 16px !important;
        }
        
        .grid-view th, .grid-view td {
            font-size: 12px !important;
            padding: 8px !important;
        }
        
        .nav-tabs-custom .nav-link {
            font-size: 14px !important;
            padding: 10px 15px !important;
        }
        
        .modal-custom .modal-content {
            width: 95%;
            max-width: 95%;
        }
        
        .btn-save, .btn-search, .btn-back, .btn-clear {
            height: 46px !important;
            font-size: 15px !important;
        }
        
        .section-header {
            font-size: 15px !important;
            padding: 10px 14px !important;
        }
        
        .form-control {
            font-size: 14px !important;
            height: 38px !important;
            padding: 8px 12px !important;
        }
        
        .current-odometer-value {
            font-size: 26px !important;
        }
        
        .statistics-value {
            font-size: 22px !important;
        }
    }

    @media (min-width: 769px) and (max-width: 1024px) {
        .main-box {
            padding: 25px;
        }
        
        .car-letter-box { width: 58px; }
        .car-number-single-box { width: 95px; }
    }
</style>

    <div class="container mt-4" dir="rtl">
        <div class="text-center mb-4">
            <h2 class="page-title">الإدارة المتكاملة للمركبات</h2>
        </div>

        <!-- ===== Alert Messages ===== -->
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <div class="main-box">

            <!-- ===== SEARCH SECTION ===== -->
            <div class="section-header">
                <i class="fas fa-search"></i> بحث برقم السيارة
            </div>

            <div class="row mb-4">
                <div class="col-md-8">
                    <label class="form-label">رقم السيارة</label>
                    <div class="car-number-container mb-2">
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter1" runat="server"
                                CssClass="form-control car-letter"
                                MaxLength="1" placeholder="أ"
                                onkeyup="moveToNext(this, 'txtCarLetter2');"
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, ''); }"
                                onkeypress="return isArabicLetter(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter2" runat="server"
                                CssClass="form-control car-letter"
                                MaxLength="1" placeholder="ب"
                                onkeyup="moveToNext(this, 'txtCarLetter3');"
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter1'); }"
                                onkeypress="return isArabicLetter(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter3" runat="server"
                                CssClass="form-control car-letter"
                                MaxLength="1" placeholder="ج"
                                onkeyup="moveToNext(this, 'txtCarNumbers');"
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter2'); }"
                                onkeypress="return isArabicLetter(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                        <span class="car-number-separator">-</span>
                        <div class="car-number-single-box">
                            <asp:TextBox ID="txtCarNumbers" runat="server"
                                CssClass="form-control car-number-single"
                                MaxLength="4" placeholder="1234"
                                onkeypress="return isNumber(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <label class="form-label">&nbsp;</label>
                    <asp:Button ID="btnSearch" runat="server"
                        Text="بحث"
                        CssClass="btn btn-search w-100"
                        OnClick="BtnSearch_Click"
                        OnClientClick="return validateCarNumber();" />
                </div>
            </div>

            <!-- Hidden fields to maintain state -->
            <asp:HiddenField ID="hfVehicleId" runat="server" Value="0" />
            <asp:HiddenField ID="hfCarNumberId" runat="server" Value="0" />
            <asp:HiddenField ID="hfActiveTab" runat="server" Value="tabLicense" />
            <asp:HiddenField ID="HiddenField1" runat="server" Value="" />

            <!-- ===== VEHICLE INFO ===== -->
            <asp:Panel ID="pnlVehicleInfo" runat="server" Visible="false">
                <div class="car-info-card">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="car-number-display">
                                رقم السيارة: <asp:Label ID="lblCarNumber" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- ===== CURRENT ODOMETER ===== -->
            <asp:Panel ID="pnlCurrentOdometer" runat="server" Visible="false">
                <div class="current-odometer">
                    <span class="current-odometer-label">قراءة العداد الحالية (أقصى قراءة):</span>
                    <span class="current-odometer-value">
                        <asp:Label ID="lblCurrentOdometer" runat="server" Text="0"></asp:Label> كم
                    </span>
                </div>
            </asp:Panel>

            <!-- ===== TABS ===== -->
            <asp:Panel ID="pnlTabs" runat="server" Visible="false">
                <div class="nav-tabs-custom" id="mainTabs">
                    <div class="nav-item">
                        <button class="nav-link active" data-tab="tabLicense">
                            <i class="fas fa-id-card"></i> الترخيص
                            <span class="badge-count" id="licenseCount">0</span>
                        </button>
                    </div>
                    <div class="nav-item">
                        <button class="nav-link" data-tab="tabInsurance">
                            <i class="fas fa-shield-alt"></i> التأمين
                            <span class="badge-count" id="insuranceCount">0</span>
                        </button>
                    </div>
                    <div class="nav-item">
                        <button class="nav-link" data-tab="tabMaintenance">
                            <i class="fas fa-wrench"></i> الصيانة
                            <span class="badge-count" id="maintenanceCount">0</span>
                        </button>
                    </div>
                </div>

                <div class="tab-content">

                    <!-- ===== TAB 1: LICENSE ===== -->
                    <div class="tab-pane active" id="tabLicense">
            <!-- Current License -->
<div class="section-header">
    <i class="fas fa-id-card"></i> الترخيص الحالي
</div>

<asp:Panel ID="pnlCurrentLicense" runat="server" Visible="false">
    <div class="current-license-card">
        <div class="row">
            <div class="col-md-3">
                <div class="current-license-label">رقم الترخيص</div>
                <div class="current-license-value"><asp:Label ID="lblCurrentLicenseNumber" runat="server" Text="-"></asp:Label></div>
            </div>
            <div class="col-md-3">
                <div class="current-license-label">نوع الترخيص</div>
                <div class="current-license-value"><asp:Label ID="lblCurrentLicenseType" runat="server" Text="-"></asp:Label></div>
            </div>
            <div class="col-md-3">
                <div class="current-license-label">تاريخ الإصدار</div>
                <div class="current-license-value"><asp:Label ID="lblCurrentIssueDate" runat="server" Text="-"></asp:Label></div>
            </div>
            <div class="col-md-3">
                <div class="current-license-label">تاريخ الانتهاء</div>
                <div class="current-license-value"><asp:Label ID="lblCurrentExpiryDate" runat="server" Text="-"></asp:Label></div>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-4">
                <div class="current-license-label">وحدة المرور</div>
                <div class="current-license-value"><asp:Label ID="lblCurrentTrafficUnit" runat="server" Text="-"></asp:Label></div>
            </div>
            <div class="col-md-4">
                <div class="current-license-label">تكلفة التجديد</div>
                <div class="current-license-value"><asp:Label ID="lblCurrentRenewCost" runat="server" Text="-"></asp:Label></div>
            </div>
            <div class="col-md-4">
                <div class="current-license-label">رقم التسجيل</div>
                <div class="current-license-value"><asp:Label ID="lblCurrentRegistrationNumber" runat="server" Text="-"></asp:Label></div>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-md-12">
                <div class="current-license-label">ملاحظات</div>
                <div class="current-license-value"><asp:Label ID="lblCurrentNotes" runat="server" Text="-"></asp:Label></div>
            </div>
        </div>
    </div>
</asp:Panel>



                        <!-- Mandatory Insurance Info -->
                        <asp:Panel ID="pnlMandatoryInsuranceInfo" runat="server" Visible="false" CssClass="insurance-info-card">
                            <div class="row">
                                <div class="col-md-12">
                                    <strong><i class="fas fa-shield-alt"></i> التأمين الإجباري المرتبط بالترخيص</strong>
                                </div>
                                <div class="col-md-4 mt-2">
                                    <div class="insurance-label">شركة التأمين</div>
                                    <div class="insurance-value"><asp:Label ID="lblMandatoryInsuranceCompany" runat="server"></asp:Label></div>
                                </div>
                                <div class="col-md-4 mt-2">
                                    <div class="insurance-label">التكلفة</div>
                                    <div class="insurance-value"><asp:Label ID="lblMandatoryInsuranceCost" runat="server"></asp:Label></div>
                                </div>
                                <div class="col-md-4 mt-2">
                                    <div class="insurance-label">ملاحظات</div>
                                    <div class="insurance-value"><asp:Label ID="lblMandatoryInsuranceNotes" runat="server"></asp:Label></div>
                                </div>
                            </div>
                        </asp:Panel>

                        <!-- License Warning -->
                        <asp:Panel ID="pnlActiveLicenseWarning" runat="server" Visible="false" CssClass="alert-warning-custom">
                            <i class="fas fa-exclamation-triangle"></i>
                            <asp:Label ID="lblActiveLicenseWarning" runat="server"></asp:Label>
                        </asp:Panel>

                        <!-- License Form -->
                        <asp:Panel ID="pnlLicenseForm" runat="server" Visible="false">
                            <div class="section-header">
                                <i class="fas fa-plus-circle"></i>
                                <asp:Label ID="lblFormTitle" runat="server" Text="إضافة ترخيص جديد"></asp:Label>
                            </div>
                            <div class="row g-3">
                                <div class="col-md-4">
                                    <label class="form-label required-field">رقم الترخيص</label>
                                    <asp:TextBox ID="txtLicenseNumber" runat="server" CssClass="form-control" MaxLength="50" placeholder="مثال: 123456789"></asp:TextBox>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label required-field">نوع الترخيص</label>
                                    <asp:DropDownList ID="ddlLicenseType" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label required-field">تاريخ الإصدار</label>
                                    <asp:TextBox ID="txtIssueDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label required-field">تاريخ الانتهاء</label>
                                    <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">وحدة المرور</label>
                                    <asp:DropDownList ID="ddlTrafficUnit" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">تكلفة التجديد</label>
                                    <asp:TextBox ID="txtRenewCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">رقم التسجيل</label>
                                    <asp:TextBox ID="txtRegistrationNumber" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">تاريخ الفحص الفني</label>
                                    <asp:TextBox ID="txtTechInspectionDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">انتهاء الفحص الفني</label>
                                    <asp:TextBox ID="txtTechInspectionExpiry" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-12">
                                    <label class="form-label">ملاحظات الترخيص</label>
                                    <asp:TextBox ID="txtLicenseNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="ملاحظات إضافية عن الترخيص"></asp:TextBox>
                                </div>

                                <!-- Mandatory Insurance Section -->
                                <div class="col-md-12 mt-3">
                                    <hr />
                                    <h5 class="text-primary"><i class="fas fa-shield-alt"></i> بيانات التأمين الإجباري</h5>
                                    <small class="text-muted">التأمين الإجباري يرتبط بالترخيص ويبدأ وينتهي معه</small>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label required-field">شركة التأمين الإجباري</label>
                                    <asp:DropDownList ID="ddlMandatoryInsurance" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">تكلفة التأمين الإجباري</label>
                                    <asp:TextBox ID="txtMandatoryInsuranceCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">ملاحظات التأمين</label>
                                    <asp:TextBox ID="txtMandatoryInsuranceNotes" runat="server" CssClass="form-control" placeholder="ملاحظات عن التأمين الإجباري"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mt-4">
                                <div class="col-md-6">
                                    <asp:Button ID="btnSaveLicense" runat="server" Text="حفظ الترخيص والتأمين" CssClass="btn btn-save" OnClick="BtnSaveLicense_Click" OnClientClick="return validateLicenseForm();" />
                                </div>
                                <div class="col-md-6">
                                    <asp:Button ID="btnCancelForm" runat="server" Text="إلغاء" CssClass="btn btn-back" OnClick="BtnCancelForm_Click" CausesValidation="false" />
                                </div>
                            </div>
                        </asp:Panel>
              

                        <!-- License History -->
                        <div class="mt-4">
                            <div class="section-header">
                                <i class="fas fa-history"></i> سجل التراخيص السابقة
                                <asp:Button ID="btnNewLicense" runat="server" Text="+ ترخيص جديد" CssClass="btn btn-renew float-left" OnClick="BtnNewLicense_Click" CausesValidation="false" Visible="false" />
                            </div>
                            <asp:GridView ID="gvLicenseHistory" runat="server"
                                CssClass="grid-view"
                                AutoGenerateColumns="false"
                                EmptyDataText="لا توجد سجلات تراخيص سابقة"
                                AllowPaging="true" PageSize="10"
                                OnPageIndexChanging="GvLicenseHistory_PageIndexChanging"
                                OnRowDataBound="GvLicenseHistory_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="license_number" HeaderText="رقم الترخيص" />
                                    <asp:BoundField DataField="license_type" HeaderText="نوع الترخيص" />
                                    <asp:BoundField DataField="issue_date" HeaderText="تاريخ الإصدار" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="expiry_date" HeaderText="تاريخ الانتهاء" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="traffic_unit" HeaderText="وحدة المرور" />
                                    <asp:BoundField DataField="renewal_cost" HeaderText="تكلفة التجديد" DataFormatString="{0:N2}" />
                                    <asp:BoundField DataField="mandatory_insurance" HeaderText="التأمين الإجباري" />
                                    <asp:TemplateField HeaderText="الحالة">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus" runat="server"
                                                CssClass='<%# GetStatusCss(Convert.ToDateTime(Eval("expiry_date"))) %>'
                                                Text='<%# GetStatusText(Convert.ToDateTime(Eval("expiry_date"))) %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="إجراءات">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkView" runat="server" CssClass="btn btn-sm btn-info" OnClick="ViewLicense_Click" CommandArgument='<%# Eval("license_id") %>'><i class="fas fa-eye"></i> عرض</asp:LinkButton>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-danger" OnClick="DeleteLicense_Click" CommandArgument='<%# Eval("license_id") %>' OnClientClick="return confirm('هل أنت متأكد من حذف هذا الترخيص؟');"><i class="fas fa-trash"></i> حذف</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                            </asp:GridView>
                        </div>
                    </div>

                    <!-- ===== TAB 2: INSURANCE ===== -->
                    <div class="tab-pane" id="tabInsurance">
                        <div class="section-header">
                            <i class="fas fa-shield-alt"></i> إضافة تأمين تكاملي
                        </div>

                        <div class="row g-3">
                            <div class="col-md-6">
                                <label class="form-label required-field">شركة التأمين</label>
                                <asp:DropDownList ID="ddlInsuranceCompany" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label required-field">التكلفة</label>
                                <asp:TextBox ID="txtInsuranceCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label required-field">تاريخ البداية</label>
                                <asp:TextBox ID="txtInsuranceStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label required-field">تاريخ النهاية</label>
                                <asp:TextBox ID="txtInsuranceEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">ملاحظات</label>
                                <asp:TextBox ID="txtInsuranceNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="col-md-6">
                                <asp:Button ID="btnSaveInsurance" runat="server" Text="حفظ التأمين التكاملي" CssClass="btn btn-save" OnClick="BtnSaveInsurance_Click" />
                            </div>
                            <div class="col-md-6">
                                <asp:Button ID="btnClearInsurance" runat="server" Text="رجوع" CssClass="btn btn-back" OnClick="BtnClearInsurance_Click" />
                            </div>
                        </div>

                        <div class="mt-4">
                            <div class="section-header">
                                <i class="fas fa-history"></i> سجل التأمينات السابقة
                            </div>
                            <asp:GridView ID="gvInsuranceHistory" runat="server"
                                CssClass="grid-view"
                                AutoGenerateColumns="false"
                                EmptyDataText="لا توجد تأمينات سابقة"
                                AllowPaging="true" PageSize="5"
                                OnPageIndexChanging="GvInsuranceHistory_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="company_name" HeaderText="الشركة" />
                                    <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="cost" HeaderText="التكلفة" DataFormatString="{0:N2}" />
                                    <asp:BoundField DataField="notes" HeaderText="ملاحظات" />
                                    <asp:TemplateField HeaderText="الحالة">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus" runat="server"
                                                CssClass='<%# Convert.ToDateTime(Eval("end_date")) > DateTime.Now ? "status-active" : "status-expired" %>'
                                                Text='<%# Convert.ToDateTime(Eval("end_date")) > DateTime.Now ? "ساري" : "منتهي" %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                            </asp:GridView>
                        </div>
                    </div>

                    <!-- ===== TAB 3: MAINTENANCE ===== -->
                    <div class="tab-pane" id="tabMaintenance">
                        <!-- Engine Overhaul Section -->
                        <asp:Panel ID="pnlEngineOverhaul" runat="server" Visible="false">
                            <div class="engine-overhaul-card">
                                <div class="section-header" style="background: linear-gradient(90deg, #ff9800, #ffffff);">
                                    <i class="fas fa-oil-can"></i> عمرة الموتور
                                </div>

                                <div class="overhaul-status" id="divOverhaulStatus" runat="server" style="margin-bottom: 20px;">
                                    <asp:Label ID="lblOverhaulStatus" runat="server" Font-Bold="true" Font-Size="16px"></asp:Label>
                                </div>

                                <div class="row g-3">
                                    <div class="col-md-3">
                                        <label class="form-label">تاريخ عمرة الموتور</label>
                                        <asp:TextBox ID="txtEngineOverhaulDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <small class="text-muted">اختر تاريخ إجراء العمرة</small>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="form-label">قراءة العداد عند العمرة</label>
                                        <asp:TextBox ID="txtEngineOverhaulOdometer" runat="server" CssClass="form-control" TextMode="Number" step="1" min="0" placeholder="كم"></asp:TextBox>
                                        <small class="text-muted">أدخل قراءة العداد عند إجراء العمرة</small>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="form-label">تكلفة العمرة</label>
                                        <asp:TextBox ID="txtEngineOverhaulCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" placeholder="0.00"></asp:TextBox>
                                        <small class="text-muted">أدخل تكلفة عمرة الموتور</small>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="form-label">ملاحظات العمرة</label>
                                        <asp:TextBox ID="txtEngineOverhaulNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="1" placeholder="ملاحظات عن عمرة الموتور"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="row mt-3">
                                    <div class="col-md-6">
                                        <asp:Button ID="btnSaveEngineOverhaul" runat="server"
                                            Text="تسجيل عمرة الموتور"
                                            CssClass="btn btn-overhaul w-100"
                                            OnClick="BtnSaveEngineOverhaul_Click"
                                            OnClientClick="return validateEngineOverhaul();" />
                                    </div>
                                    <div class="col-md-6">
                                        <asp:Button ID="btnClearEngineOverhaul" runat="server"
                                            Text="مسح بيانات العمرة"
                                            CssClass="btn btn-clear-overhaul w-100"
                                            OnClick="BtnClearEngineOverhaul_Click"
                                            OnClientClick="return confirm('هل أنت متأكد من مسح بيانات عمرة الموتور؟');" />
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>

       

                        <!-- Maintenance Form -->
                        <div class="section-header">
                            <i class="fas fa-wrench"></i> إضافة صيانة جديدة
                        </div>

                        <div class="row g-3">
                            <div class="col-md-4">
                                <label class="form-label">نوع الصيانة</label>
                                <asp:DropDownList ID="ddlMaintenanceType" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر نوع الصيانة --</asp:ListItem>
                                    <asp:ListItem Value="صيانة دورية">صيانة دورية</asp:ListItem>
                                    <asp:ListItem Value="اعطال">اعطال</asp:ListItem>
                                    <asp:ListItem Value="صيانة طارئة">صيانة طارئة</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">تاريخ الصيانة</label>
                                <asp:TextBox ID="txtMaintenanceDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">قراءة العداد</label>
                                <asp:TextBox ID="txtOdometerReading" runat="server" CssClass="form-control" TextMode="Number" step="1" min="0" placeholder="كم"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">التكلفة</label>
                                <asp:TextBox ID="txtMaintenanceCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" placeholder="0.00"></asp:TextBox>
                            </div>
                            <div class="col-md-9">
                                <label class="form-label">ملاحظات</label>
                                <asp:TextBox ID="txtMaintenanceNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="ملاحظات إضافية عن الصيانة"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Multiple Parts Selection -->
                        <div class="row mt-3">
                            <div class="col-12">
                                <label class="form-label">قطع الغيار</label>
                                <div class="parts-container">
                                    <div id="partRowsContainer">
                                        <!-- Part rows will be added here dynamically -->
                                    </div>
                                    <button type="button" class="btn btn-sm btn-add-part" onclick="addPartRow()">
                                        <i class="fas fa-plus"></i> إضافة قطعة غيار
                                    </button>
                                </div>
                                <asp:HiddenField ID="hfPartsData" runat="server" />
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="col-md-6">
                                <asp:Button ID="btnSaveMaintenance" runat="server" Text="حفظ الصيانة" CssClass="btn btn-save" OnClick="BtnSaveMaintenance_Click" OnClientClick="return collectPartsData() && validateOdometerBeforeSave();" />
                            </div>
                            <div class="col-md-6">
                                <asp:Button ID="btnClearMaintenance" runat="server" Text="عرض سجل الصيانة الكامل" CssClass="btn btn-back" OnClick="BtnClearMaintenance_Click" />
                            </div>
                        </div>
                                        <!-- Statistics -->
                <asp:Panel ID="pnlStatistics" runat="server" Visible="false">
                    <div class="row mt-3">
                        <div class="col-md-6">
                            <div class="statistics-card">
                                <h5>إجمالي تكاليف الصيانة</h5>
                                <div class="statistics-value">
                                    <asp:Label ID="lblTotalCost" runat="server" Text="0"></asp:Label> ج.م
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="statistics-card">
                                <h5>عدد مرات الصيانة</h5>
                                <div class="statistics-value" style="color: #007bff;">
                                    <asp:Label ID="lblMaintenanceCount" runat="server" Text="0"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                        <div class="mt-4">
                            <div class="section-header">
                                <i class="fas fa-history"></i> سجل الصيانة السابقة
                            </div>
                            <asp:GridView ID="gvMaintenanceHistory" runat="server"
                                CssClass="grid-view"
                                AutoGenerateColumns="false"
                                EmptyDataText="لا توجد سجلات صيانة لهذه السيارة"
                                AllowPaging="true" PageSize="5"
                                OnPageIndexChanging="GvMaintenanceHistory_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="ServiceDate" HeaderText="تاريخ الصيانة" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="False" />
                                    <asp:BoundField DataField="MaintenanceType" HeaderText="نوع الصيانة" />
                                    <asp:BoundField DataField="PartsNames" HeaderText="قطع الغيار" />
                                    <asp:BoundField DataField="OdometerReading" HeaderText="قراءة العداد" ItemStyle-CssClass="km-column" />
                                    <asp:BoundField DataField="Cost" HeaderText="التكلفة" DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
                                </Columns>
                                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                            </asp:GridView>
                        </div>
                    </div>

                </div><!-- /.tab-content -->
            </asp:Panel>

        </div><!-- /.main-box -->
    </div>

    <!-- ===== MODAL ===== -->
    <div id="detailsModal" class="modal-custom">
        <div class="modal-content">
            <div class="modal-header">
                <h5 id="modalTitle"><i class="fas fa-info-circle"></i> التفاصيل</h5>
                <button class="close" onclick="closeModal()">&times;</button>
            </div>
            <div class="modal-body" id="modalBody"></div>
            <div class="modal-footer">
                <button class="btn-secondary" onclick="closeModal()">إغلاق</button>
            </div>
        </div>
    </div>

    <!-- ===== SCRIPTS ===== -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <script type="text/javascript">
        // ===== Car Number Functions =====
        function moveToNext(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) nextField.focus();
            }
        }

        function moveToPrevious(current, prevFieldId) {
            if (current.value.length === 0 && prevFieldId) {
                var prevField = document.getElementById(prevFieldId);
                if (prevField) prevField.focus();
            }
        }

        function isArabicLetter(event) {
            var charCode = event.which || event.keyCode;
            var charStr = String.fromCharCode(charCode);
            var arabicPattern = /[\u0600-\u06FF]/;
            if (charCode === 8 || charCode === 9 || charCode === 13 ||
                charCode === 37 || charCode === 39 || charCode === 46) return true;
            return arabicPattern.test(charStr);
        }

        function isNumber(event) {
            var charCode = event.which || event.keyCode;
            if (charCode >= 48 && charCode <= 57) return true;
            if (charCode === 8 || charCode === 9 || charCode === 13 ||
                charCode === 37 || charCode === 39 || charCode === 46) return true;
            var arabicNumberPattern = /[\u0660-\u0669]/;
            return arabicNumberPattern.test(String.fromCharCode(charCode));
        }

        function validateCarNumber() {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');
            if (!letter1.value || !numbers.value) {
                alert('❌ الرجاء إدخال رقم السيارة كاملاً');
                return false;
            }
            return true;
        }

        // ===== Tab Switching =====
        document.addEventListener('DOMContentLoaded', function () {
            var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
            var activeTab = document.getElementById('<%= hfActiveTab.ClientID %>');

    // Restore active tab from hidden field
    if (activeTab && activeTab.value) {
        tabs.forEach(function (tab) {
            tab.classList.remove('active');
            var tabId = tab.getAttribute('data-tab');
            if (tabId === activeTab.value) {
                tab.classList.add('active');
            }
        });

        // Show the correct pane
        var panes = document.querySelectorAll('.tab-pane');
        panes.forEach(function (p) { p.classList.remove('active'); });
        var targetPane = document.getElementById(activeTab.value);
        if (targetPane) targetPane.classList.add('active');
    }

    tabs.forEach(function (tab) {
        tab.addEventListener('click', function (e) {
            e.preventDefault();
            // Remove active from all tabs
            tabs.forEach(function (t) { t.classList.remove('active'); });
            this.classList.add('active');

            // Hide all panes
            var panes = document.querySelectorAll('.tab-pane');
            panes.forEach(function (p) { p.classList.remove('active'); });

            // Show target pane
            var targetId = this.getAttribute('data-tab');
            var targetPane = document.getElementById(targetId);
            if (targetPane) targetPane.classList.add('active');

            // Save active tab to hidden field
            if (activeTab) {
                activeTab.value = targetId;
            }
        });
    });

    // ... rest of your existing code ...
});

        // ===== Modal Functions =====
        function openModal(title, html) {
            document.getElementById('modalTitle').innerHTML = title;
            document.getElementById('modalBody').innerHTML = html;
            document.getElementById('detailsModal').style.display = 'block';
        }

        function closeModal() {
            document.getElementById('detailsModal').style.display = 'none';
        }

        // Close modal on ESC
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') closeModal();
        });

        // Close modal on click outside
        document.addEventListener('click', function(e) {
            var modal = document.getElementById('detailsModal');
            if (e.target === modal) closeModal();
        });

        // ===== Parts Functions =====
        var availableParts = [];

        // ===== Parts Functions =====
        var availableParts = [];

        function loadPartsData() {
            try {
                // Get the parts data from the hidden field or ViewState
                var partsData = document.getElementById('<%= hfPartsData.ClientID %>');

        // If we have ViewState data, use it
        <% if (ViewState["PartsData"] != null) { %>
        availableParts = <%= ViewState["PartsData"] %>;
        <% } else { %>
            // Fallback: try to get from the page
            var partsJson = document.getElementById('hiddenPartsData');
            if (partsJson && partsJson.value) {
                availableParts = JSON.parse(partsJson.value);
            }
        <% } %>

                console.log('Parts loaded:', availableParts.length);
            } catch (e) {
                console.error('Error loading parts:', e);
                availableParts = [];
            }
        }

        function addPartRow(partId, partName) {
            var container = document.getElementById('partRowsContainer');
            if (!container) {
                console.error('Container not found');
                return;
            }

            var rowDiv = document.createElement('div');
            rowDiv.className = 'part-row';
            rowDiv.style.display = 'flex';
            rowDiv.style.gap = '10px';
            rowDiv.style.marginBottom = '10px';

            var select = document.createElement('select');
            select.className = 'form-control';
            select.style.flex = '1';
            select.style.height = '45px';
            select.style.fontSize = '15px';

            var emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.text = '-- اختر قطعة غيار --';
            select.appendChild(emptyOption);

            if (availableParts && availableParts.length > 0) {
                for (var i = 0; i < availableParts.length; i++) {
                    var option = document.createElement('option');
                    option.value = availableParts[i].id;
                    option.text = availableParts[i].name;
                    if (partId && availableParts[i].id == partId) {
                        option.selected = true;
                    }
                    select.appendChild(option);
                }
            } else {
                // Add a message option if no parts are available
                var noPartsOption = document.createElement('option');
                noPartsOption.value = '';
                noPartsOption.text = '-- لا توجد قطع غيار متاحة --';
                select.appendChild(noPartsOption);
            }

            var removeBtn = document.createElement('button');
            removeBtn.innerHTML = '<i class="fas fa-times"></i>';
            removeBtn.type = 'button';
            removeBtn.style.cssText = 'width: 45px; height: 45px; background-color: #dc3545; color: white; border: none; border-radius: 5px; cursor: pointer; padding: 6px; font-size: 16px;';
            removeBtn.onclick = function () {
                if (document.querySelectorAll('#partRowsContainer .part-row').length > 1) {
                    this.parentElement.remove();
                } else {
                    alert('يجب أن يكون هناك صف واحد على الأقل من قطع الغيار');
                }
            };

            rowDiv.appendChild(select);
            rowDiv.appendChild(removeBtn);
            container.appendChild(rowDiv);
        }

        // Initialize parts on page load
        document.addEventListener('DOMContentLoaded', function () {
            loadPartsData();

            // Add initial part row if container is empty
            var container = document.getElementById('partRowsContainer');
            if (container && container.children.length === 0) {
                addPartRow();
            }
        });
        function collectPartsData() {
            var partRows = document.querySelectorAll('#partRowsContainer .part-row');
            var partsIds = [];
            var partsNames = [];

            for (var i = 0; i < partRows.length; i++) {
                var select = partRows[i].querySelector('select');
                if (select && select.value) {
                    partsIds.push(select.value);
                    // Get the selected text (part name)
                    var selectedOption = select.options[select.selectedIndex];
                    if (selectedOption) {
                        partsNames.push(selectedOption.text);
                    }
                }
            }

            var hfPartsData = document.getElementById('<%= hfPartsData.ClientID %>');
            if (hfPartsData) {
                hfPartsData.value = partsIds.join(',');
            }

            // Also store part names in a hidden field if needed
            var hfPartsNames = document.getElementById('hfPartsNames');
            if (hfPartsNames) {
                hfPartsNames.value = partsNames.join(', ');
            }

            return true;
        }

        // ===== Validation Functions =====
        function validateLicenseForm() {
            var licenseNumber = document.getElementById('<%= txtLicenseNumber.ClientID %>');
            var licenseType = document.getElementById('<%= ddlLicenseType.ClientID %>');
            var issueDate = document.getElementById('<%= txtIssueDate.ClientID %>');
            var expiryDate = document.getElementById('<%= txtExpiryDate.ClientID %>');

            if (!licenseNumber.value) { alert('❌ الرجاء إدخال رقم الترخيص'); licenseNumber.focus(); return false; }
            if (licenseType.value === '') { alert('❌ الرجاء اختيار نوع الترخيص'); licenseType.focus(); return false; }
            if (!issueDate.value) { alert('❌ الرجاء إدخال تاريخ الإصدار'); issueDate.focus(); return false; }
            if (!expiryDate.value) { alert('❌ الرجاء إدخال تاريخ الانتهاء'); expiryDate.focus(); return false; }

            var issue = new Date(issueDate.value);
            var expiry = new Date(expiryDate.value);
            if (expiry <= issue) {
                alert('❌ تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار');
                expiryDate.focus();
                return false;
            }
            return true;
        }

        function validateOdometerBeforeSave() {
            var currentOdometerLabel = document.getElementById('<%= lblCurrentOdometer.ClientID %>');
            var newOdometerField = document.getElementById('<%= txtOdometerReading.ClientID %>');

            if (!currentOdometerLabel || !newOdometerField) return true;

            var currentOdometerText = currentOdometerLabel.innerText || currentOdometerLabel.textContent;
            var maxWorkOrderKM = parseInt(currentOdometerText.replace(/[^0-9]/g, ''), 10);
            var newOdo = parseInt(newOdometerField.value, 10);

            if (isNaN(newOdo) || newOdo <= 0) {
                alert('الرجاء إدخال قراءة عداد صحيحة أكبر من صفر');
                newOdometerField.style.backgroundColor = '#ffebee';
                newOdometerField.focus();
                return false;
            }

            if (newOdo > maxWorkOrderKM) {
                alert('❌ خطأ: قراءة العداد المدخلة (' + newOdo.toLocaleString() +
                    ' كم) لا يمكن أن تتجاوز آخر قراءة مسجلة (' + maxWorkOrderKM.toLocaleString() + ' كم)');
                newOdometerField.style.backgroundColor = '#ffebee';
                newOdometerField.focus();
                return false;
            }

            newOdometerField.style.backgroundColor = '';
            return confirm('هل أنت متأكد من حفظ بيانات الصيانة؟\n\nقراءة العداد: ' + newOdo.toLocaleString() + ' كم');
        }

        function validateEngineOverhaul() {
            var overhaulDate = document.getElementById('<%= txtEngineOverhaulDate.ClientID %>');
            var overhaulOdometer = document.getElementById('<%= txtEngineOverhaulOdometer.ClientID %>');
            var currentOdometerLabel = document.getElementById('<%= lblCurrentOdometer.ClientID %>');

            if (!overhaulDate.value) {
                alert('الرجاء إدخال تاريخ عمرة الموتور');
                overhaulDate.focus();
                return false;
            }

            if (!overhaulOdometer.value || parseInt(overhaulOdometer.value) <= 0) {
                alert('الرجاء إدخال قراءة عداد صحيحة لعمرة الموتور');
                overhaulOdometer.focus();
                return false;
            }

            var overhaulCost = document.getElementById('<%= txtEngineOverhaulCost.ClientID %>');
            if (overhaulCost && overhaulCost.value) {
                if (parseFloat(overhaulCost.value) < 0) {
                    alert('الرجاء إدخال تكلفة صحيحة (أكبر من أو تساوي صفر)');
                    overhaulCost.focus();
                    return false;
                }
            }

            if (currentOdometerLabel) {
                var currentOdometerText = currentOdometerLabel.innerText || currentOdometerLabel.textContent;
                var currentKM = parseInt(currentOdometerText.replace(/[^0-9]/g, ''), 10);
                var overhaulKM = parseInt(overhaulOdometer.value, 10);

                if (overhaulKM > currentKM) {
                    alert('❌ خطأ: قراءة العداد المدخلة للعمرة (' + overhaulKM.toLocaleString() +
                        ' كم) لا يمكن أن تتجاوز آخر قراءة مسجلة (' + currentKM.toLocaleString() + ' كم)');
                    overhaulOdometer.style.backgroundColor = '#ffebee';
                    overhaulOdometer.focus();
                    return false;
                }
            }

            overhaulOdometer.style.backgroundColor = '';
            return confirm('هل أنت متأكد من تسجيل عمرة الموتور؟\n\nالتاريخ: ' +
                overhaulDate.value + '\nالعداد: ' + parseInt(overhaulOdometer.value).toLocaleString() + ' كم');
        }
        // ===== View License Modal Functions =====
        function showLicenseDetailsWithInsurance(data) {
            var html = '';

            // License Information
            html += '<div class="license-details-section">';
            html += '<h5 style="color: #a50d25; border-bottom: 2px solid #a50d25; padding-bottom: 10px; margin-bottom: 15px;">📋 معلومات الترخيص</h5>';
            html += '<div class="row">';
            html += '<div class="col-md-6"><strong>رقم الترخيص:</strong> ' + data.license_number + '</div>';
            html += '<div class="col-md-6"><strong>نوع الترخيص:</strong> ' + data.license_type + '</div>';
            html += '<div class="col-md-6"><strong>تاريخ الإصدار:</strong> ' + data.issue_date + '</div>';
            html += '<div class="col-md-6"><strong>تاريخ الانتهاء:</strong> ' + data.expiry_date + '</div>';
            html += '<div class="col-md-6"><strong>وحدة المرور:</strong> ' + data.traffic_unit + '</div>';
            html += '<div class="col-md-6"><strong>تكلفة التجديد:</strong> ' + data.renewal_cost + ' ج.م</div>';
            html += '<div class="col-md-6"><strong>رقم التسجيل:</strong> ' + data.registration_number + '</div>';
            html += '</div>';

            // Technical Inspection
            html += '<div class="row mt-2">';
            html += '<div class="col-md-6"><strong>تاريخ الفحص الفني:</strong> ' + (data.tech_inspection_date || '-') + '</div>';
            html += '<div class="col-md-6"><strong>انتهاء الفحص الفني:</strong> ' + (data.tech_inspection_expiry || '-') + '</div>';
            html += '</div>';

            // Notes
            if (data.notes && data.notes !== '-') {
                html += '<div class="row mt-2">';
                html += '<div class="col-md-12"><strong>ملاحظات:</strong> ' + data.notes + '</div>';
                html += '</div>';
            }
            html += '</div>';

            // Mandatory Insurance Information
            if (data.mandatory_insurance_company && data.mandatory_insurance_company !== '-') {
                html += '<div class="insurance-details-section mt-4" style="background: #e8f4f8; padding: 15px; border-radius: 8px; border-right: 4px solid #17a2b8;">';
                html += '<h6 style="color: #17a2b8; margin-bottom: 10px;"><i class="fas fa-shield-alt"></i> التأمين الإجباري المرتبط بالترخيص</h6>';
                html += '<div class="row">';
                html += '<div class="col-md-6"><strong>شركة التأمين:</strong> ' + data.mandatory_insurance_company + '</div>';
                html += '<div class="col-md-6"><strong>التكلفة:</strong> ' + data.mandatory_insurance_cost + ' ج.م</div>';
                if (data.mandatory_insurance_start_date && data.mandatory_insurance_start_date !== '-') {
                    html += '<div class="col-md-6"><strong>تاريخ البداية:</strong> ' + data.mandatory_insurance_start_date + '</div>';
                }
                if (data.mandatory_insurance_end_date && data.mandatory_insurance_end_date !== '-') {
                    html += '<div class="col-md-6"><strong>تاريخ النهاية:</strong> ' + data.mandatory_insurance_end_date + '</div>';
                }
                if (data.mandatory_insurance_notes && data.mandatory_insurance_notes !== '-') {
                    html += '<div class="col-md-12"><strong>ملاحظات التأمين:</strong> ' + data.mandatory_insurance_notes + '</div>';
                }
                html += '</div>';
                html += '</div>';
            }

            document.getElementById('viewLicenseModalBody').innerHTML = html;
            document.getElementById('viewLicenseModal').style.display = 'block';
        }

        function closeViewModal() {
            document.getElementById('viewLicenseModal').style.display = 'none';
        }

        // Close modal on ESC
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                closeViewModal();
            }
        });


        // Close modal on click outside
        document.addEventListener('click', function (e) {
            var modal = document.getElementById('viewLicenseModal');
            if (e.target === modal) {
                closeViewModal();
            }

        });
        
    </script>
    <!-- ===== VIEW LICENSE MODAL ===== -->
<div id="viewLicenseModal" class="modal-custom">
    <div class="modal-content">
        <div class="modal-header">
            <h5><i class="fas fa-id-card"></i> تفاصيل الترخيص</h5>
            <button class="close" onclick="closeViewModal()">&times;</button>
        </div>
        <div class="modal-body" id="viewLicenseModalBody">
            <!-- Content will be loaded here -->
        </div>
        <div class="modal-footer">
            <button class="btn-secondary" onclick="closeViewModal()">إغلاق</button>
        </div>
    </div>
</div>
</asp:Content>