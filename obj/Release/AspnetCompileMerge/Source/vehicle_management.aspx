<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="vehicle_management.aspx.cs"
    Inherits="car_WebApplication1.vehicle_management" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Add these styles to your existing CSS */

/* Common header styling for all tables - Red theme matching the save button */
.table th,
.grid-view th,
[id$="gvInsurance"] th,
[id$="gvMaintenance"] th,
[id$="gvSubscription"] th,
[id$="gvRoutes"] th {
    background-color:#a50d25 !important;
    color: white !important;
    font-weight: bold !important;
    padding: 12px !important;
    text-align: center !important;
    border-bottom: 2px solid #a50d25 !important;
}

/* Header hover effect */
[id$="gvInsurance"] th:hover,
[id$="gvMaintenance"] th:hover,
[id$="gvSubscription"] th:hover,
[id$="gvRoutes"] th:hover {
    background-color: #a50d25 !important;
}

/* Table row hover effects */
.table-striped tbody tr:hover {
    background-color: rgba(200, 16, 46, 0.05);
    transition: all 0.3s;
}

/* Pagination styling */
.pagination-ys {
    background: #f8f9fa;
    padding: 10px;
    text-align: center;
    border-radius: 0 0 10px 10px;
}

.pagination-ys a, 
.pagination-ys span {
    display: inline-block;
    padding: 5px 10px;
    margin: 0 2px;
    border: 1px solid #dee2e6;
    border-radius: 4px;
    text-decoration: none;
    color: #a50d25;
    font-weight: bold;
    transition: all 0.3s;
}

.pagination-ys a:hover {
    background-color: #a50d25;
    color: white;
    border-color: #c8102e;
}

.pagination-ys span {
    background-color: #a50d25;
    color: white;
    border-color: #c8102e;
}
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }

        .entry-box, .search-box {
            border: 2px solid #000;
            border-radius: 14px;
            padding: 25px;
            background-color: #fff;
            margin-bottom: 20px;
        }

        .result-box {
            border: 2px solid #000;
            border-radius: 14px;
            padding: 25px;
            margin-top: 20px;
            background-color: #f9f9f9;
        }

        .form-label {
            font-weight: bold;
        }

        .form-control {
            text-align: right;
        }

        .btn-save {
            background-color: #a50d25;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-save:hover {
            background-color: #a50d25;
        }

        .btn-clear, .btn-back {
            background-color: #6c757d;
            color: #fff;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-clear:hover, .btn-back:hover {
            background-color: #5a6268;
        }

        .btn-search {
            background-color: #a50d25;
            color: #fff;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-search:hover {
            background-color: #a50d25;
        }

        .btn-tab {
            background-color: transparent;
            border: none;
            color: #495057;
            font-weight: bold;
            padding: 10px 20px;
            margin: 0 2px;
            border-radius: 0;
            cursor: pointer;
        }

        .btn-tab.active {
            color: #a50d25;
            border-bottom: 3px solid #c8102e;
            background-color: transparent;
        }

        .btn-tab:hover {
            color: #a50d25;
        }

        /* Car Number Container Styles */
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

        .car-letter-box {
            width: 60px;
        }

        .car-number-single-box {
            width: 100px;
        }

        .car-letter, .car-number-single {
            text-align: center;
            font-size: 18px;
            font-weight: bold;
            direction: ltr;
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
            font-size: 20px;
            font-weight: bold;
            color: #ce1126;
            margin: 0 5px;
        }

        .car-letter:focus, .car-number-single:focus {
            outline: none;
            box-shadow: 0 0 0 3px rgba(206, 17, 38, 0.25);
            border-color: #ce1126;
        }

        .car-letter.error, .car-number-single.error {
            border-color: #dc3545;
            background-color: #fff8f8;
        }

        .car-letter.valid, .car-number-single.valid {
            border-color: #28a745;
            background-color: #f0fff0;
        }

        /* Section Headers */
        .section-header {
            background: linear-gradient(90deg, #ce1126, #ffffff);
            padding: 10px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-weight: bold;
            color: #000;
        }

        .info-card {
            background: white;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .required-field::after {
            content: " *";
            color: #ce1126;
            font-weight: bold;
        }

        .info-label {
            font-size: 13px;
            color: #666;
            margin-bottom: 5px;
        }
        
        .info-value {
            font-size: 16px;
            font-weight: bold;
            color: #333;
        }
        
        .warning-box {
            background: #fff3cd;
            border-right: 4px solid #856404;
            padding: 10px;
            margin-top: 10px;
            border-radius: 4px;
        }
        
        /* KM Display Styles */
        .km-display-box {
            background-color: #e3f2fd;
            text-align: center;
            padding: 20px;
            border-radius: 10px;
            border: 2px solid #0d6efd;
            box-shadow: 0 2px 8px rgba(13, 110, 253, 0.2);
        }
        
        .km-label {
            font-size: 16px;
            color: #666;
            margin-bottom: 5px;
        }
        
        .km-value {
            font-size: 48px;
            font-weight: bold;
            line-height: 1.2;
        }
        
        .km-unit {
            font-size: 18px;
            opacity: 0.8;
        }
        
        .km-date {
            font-size: 14px;
            color: #666;
            margin-top: 5px;
        }
        
        .km-icon {
            font-size: 24px;
            margin-bottom: 10px;
        }
        
        /* Section Buttons */
        .section-buttons {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            margin-bottom: 20px;
        }
        
        .btn-section {
            background-color: #343a40;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 8px;
            font-weight: bold;
            cursor: pointer;
            transition: all 0.3s;
        }
        
        .btn-section:hover {
            background-color: #23272b;
            transform: translateY(-2px);
        }
        
        .btn-section.active {
            background-color: #a50d25;
        }
        
        .btn-section i {
            margin-left: 8px;
        }
        
        .section-container {
            display: none;
            animation: fadeIn 0.5s;
        }
        
        .section-container.active {
            display: block;
        }
        
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(10px); }
            to { opacity: 1; transform: translateY(0); }
        }
        
        .badge-count {
            background-color:#a50d25;
            color: white;
            border-radius: 50%;
            padding: 2px 8px;
            margin-right: 5px;
            font-size: 12px;
        }

        /* GridView Styles */
        .table {
            width: 100%;
            margin-bottom: 1rem;
            background-color: transparent;
            border-collapse: collapse;
        }
        
        .table th {
            background-color: #343a40;
            color: white;
            padding: 12px;
            font-weight: bold;
            text-align: center;
        }
        
        .table td {
            padding: 10px;
            text-align: center;
            border-bottom: 1px solid #dee2e6;
        }
        
        .table-striped tbody tr:nth-of-type(odd) {
            background-color: rgba(0,0,0,.05);
        }
        
        .table-bordered {
            border: 1px solid #dee2e6;
        }

        .status-active {
            color: #28a745;
            font-weight: bold;
        }

        .status-expired {
            color: #dc3545;
            font-weight: bold;
        }

        .cost-column {
            color: #a50d25;
            font-weight: bold;
        }

        .km-column {
            color: #0d6efd;
            font-weight: bold;
        }
        
        /* Validation Styles */
        .chassis-valid {
            border-color: #28a745 !important;
            background-color: #f0fff0 !important;
        }
        
        .chassis-invalid {
            border-color: #dc3545 !important;
            background-color: #fff8f8 !important;
        }
        
        .validation-message {
            font-size: 12px;
            margin-top: 5px;
        }
        
        .validation-message.error {
            color: #dc3545;
        }
        
        .validation-message.success {
            color: #28a745;
        }
        
        .validation-message.info {
            color: #0d6efd;
        }
        
        @media (max-width: 768px) {
            .car-letter-box {
                width: 50px;
            }

            .car-number-single-box {
                width: 80px;
            }

            .car-number-separator {
                font-size: 16px;
            }
            
            .km-value {
                font-size: 32px;
            }
            
            .section-buttons {
                flex-direction: column;
            }
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <div class="text-center mb-4">
            <h2 class="page-title">إدارة المركبات</h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <!-- Tab Buttons -->
        <div class="text-center mb-4" style="border-bottom: 2px solid #dee2e6;">
            <asp:Button ID="btnTabEntry" runat="server" Text="إدخال سيارة جديدة" CssClass="btn-tab active" OnClick="btnTabEntry_Click" CausesValidation="false" />
            <asp:Button ID="btnTabSearch" runat="server" Text="استعلام عن بيانات سيارة" CssClass="btn-tab" OnClick="btnTabSearch_Click" CausesValidation="false" />
        </div>

        <asp:MultiView ID="MainMultiView" runat="server" ActiveViewIndex="0">
            
            <!-- ========== ENTRY VIEW ========== -->
            <asp:View ID="viewEntry" runat="server">
                <div class="entry-box">

                    <!-- رقم السيارة -->
                    <div class="section-header">
                        <i class="fas fa-car"></i> رقم السيارة <span class="required-field"></span>
                    </div>
                    <div class="info-card">
                        <div class="row align-items-end">
                            <div class="col-md-12">
                                <label class="form-label required-field">رقم السيارة</label>
                                <div class="car-number-container mb-2">
                                    <div class="car-letter-box">
                                        <asp:TextBox ID="txtCarLetter1" runat="server"
                                            CssClass="form-control car-letter"
                                            MaxLength="1"
                                            placeholder="أ"
                                            onkeyup="moveToNext(this, 'txtCarLetter2');"
                                            onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, ''); }"
                                            onkeypress="return isArabicLetter(event);"
                                            autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="car-letter-box">
                                        <asp:TextBox ID="txtCarLetter2" runat="server"
                                            CssClass="form-control car-letter"
                                            MaxLength="1"
                                            placeholder="ب"
                                            onkeyup="moveToNext(this, 'txtCarLetter3');"
                                            onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter1'); }"
                                            onkeypress="return isArabicLetter(event);"
                                            autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="car-letter-box">
                                        <asp:TextBox ID="txtCarLetter3" runat="server"
                                            CssClass="form-control car-letter"
                                            MaxLength="1"
                                            placeholder="ج"
                                            onkeyup="moveToNext(this, 'txtCarNumbers');"
                                            onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter2'); }"
                                            onkeypress="return isArabicLetter(event);"
                                            autocomplete="off"></asp:TextBox>
                                    </div>
                                    <span class="car-number-separator">-</span>
                                    <div class="car-number-single-box">
                                        <asp:TextBox ID="txtCarNumbers" runat="server"
                                            CssClass="form-control car-number-single"
                                            MaxLength="4"
                                            placeholder="1234"
                                            onkeypress="return isNumber(event);"
                                            autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div>
                                    <small class="text-muted">الصيغة: حرف + حرف + حرف (اختياري) + ١-٤ أرقام</small>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- البيانات الأساسية -->
                    <div class="section-header">
                        <i class="fas fa-info-circle"></i> البيانات الأساسية
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-4">
                                <label class="form-label required-field">نوع السيارة</label>
                                <asp:DropDownList ID="ddlCarType" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر نوع السيارة --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">الموديل</label>
                                <asp:DropDownList ID="ddlCarModel" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر الموديل --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">سنة الصنع</label>
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر السنة --</asp:ListItem>
                                </asp:DropDownList>
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
                                <label class="form-label required-field">عدد الركاب</label>
                                <asp:TextBox ID="txtPassengers" runat="server" CssClass="form-control" TextMode="Number" min="1"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- البيانات الفنية -->
                    <div class="section-header">
                        <i class="fas fa-cogs"></i> البيانات الفنية
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label class="form-label required-field">رقم الشاسيه</label>
                                <asp:TextBox ID="txtChassis" runat="server" 
                                    CssClass="form-control" 
                                    MaxLength="17"
                                    onkeyup="this.value = this.value.toUpperCase(); validateChassisRealTime();"
                                    onblur="validateChassisRealTime();"
                                    placeholder="مثال: 1HGCM82633A123456"></asp:TextBox>
                                <div id="chassisValidationMessage" class="validation-message"></div>
                                <small class="text-muted">رقم الشاسيه: 17 رمزاً (أرقام 0-9 وحروف A-Z ما عدا I, O)</small>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label required-field">رقم الموتور</label>
                                <asp:TextBox ID="txtMotor" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- بيانات الترخيص -->
                    <div class="section-header">
                        <i class="fas fa-id-card"></i> بيانات الترخيص
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-3">
                                <label class="form-label required-field">بداية الترخيص</label>
                                <asp:TextBox ID="txtLicenseStart" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label required-field">نهاية الترخيص</label>
                                <asp:TextBox ID="txtLicenseEnd" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label required-field">وحدة المرور</label>
                                <asp:DropDownList ID="ddlTrafficUnit" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر وحدة المرور --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">تكلفة التجديد</label>
                                <asp:TextBox ID="txtRenewCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label required-field">نوع الترخيص</label>
                                <asp:DropDownList ID="ddlLicenseType" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">-- اختر نوع الترخيص --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <!-- قراءة العداد -->
                    <div class="section-header">
                        <i class="fas fa-tachometer-alt"></i> قراءة العداد
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-4">
                                <label class="form-label">قراءة العداد الحالية (كم)</label>
                                <asp:TextBox ID="txtCurrentOdometer" runat="server" 
                                    CssClass="form-control" 
                                    TextMode="Number" 
                                    step="1" 
                                    min="0"
                                    placeholder="أدخل قراءة العداد"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">تاريخ القراءة</label>
                                <asp:TextBox ID="txtOdometerDate" runat="server" 
                                    CssClass="form-control" 
                                    TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">ملاحظات</label>
                                <asp:TextBox ID="txtOdometerNotes" runat="server" 
                                    CssClass="form-control" 
                                    placeholder="ملاحظات إضافية"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-12">
                                <small class="text-muted">
                                    <i class="fas fa-info-circle"></i> 
                                    أدخل قراءة العداد الحالية للسيارة. سيتم استخدام هذه القراءة كنقطة بداية للرحلات المستقبلية.
                                </small>
                            </div>
                        </div>
                    </div>

                    <!-- Buttons -->
                    <div class="row mt-4">
                        <div class="col-md-6">
                            <asp:Button ID="btnSave" runat="server" Text="حفظ البيانات" CssClass="btn btn-save" OnClick="btnSave_Click" OnClientClick="return validateEntryForm();" />
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnClear" runat="server" Text="مسح الحقول" CssClass="btn btn-clear" OnClick="btnClear_Click" />
                        </div>
                    </div>

                </div>
            </asp:View>

            <!-- ========== SEARCH VIEW ========== -->
            <asp:View ID="viewSearch" runat="server">
                <div class="search-box mb-4">
                    <div class="row align-items-end">
                        <div class="col-md-8">
                            <label class="form-label">رقم السيارة</label>
                            <div class="car-number-container mb-2">
                                <div class="car-letter-box">
                                    <asp:TextBox ID="txtSearchLetter1" runat="server"
                                        CssClass="form-control car-letter"
                                        MaxLength="1"
                                        placeholder="أ"
                                        onkeyup="moveToNextSearch(this, 'txtSearchLetter2');"
                                        onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPreviousSearch(this, ''); }"
                                        onkeypress="return isArabicLetter(event);"
                                        autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="car-letter-box">
                                    <asp:TextBox ID="txtSearchLetter2" runat="server"
                                        CssClass="form-control car-letter"
                                        MaxLength="1"
                                        placeholder="ب"
                                        onkeyup="moveToNextSearch(this, 'txtSearchLetter3');"
                                        onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPreviousSearch(this, 'txtSearchLetter1'); }"
                                        onkeypress="return isArabicLetter(event);"
                                        autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="car-letter-box">
                                    <asp:TextBox ID="txtSearchLetter3" runat="server"
                                        CssClass="form-control car-letter"
                                        MaxLength="1"
                                        placeholder="ج"
                                        onkeyup="moveToNextSearch(this, 'txtSearchNumbers');"
                                        onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPreviousSearch(this, 'txtSearchLetter2'); }"
                                        onkeypress="return isArabicLetter(event);"
                                        autocomplete="off"></asp:TextBox>
                                </div>
                                <span class="car-number-separator">-</span>
                                <div class="car-number-single-box">
                                    <asp:TextBox ID="txtSearchNumbers" runat="server"
                                        CssClass="form-control car-number-single"
                                        MaxLength="4"
                                        placeholder="1234"
                                        onkeyup="updateFullCarNumber();"
                                        onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPreviousSearch(this, 'txtSearchLetter3'); }"
                                        onkeypress="return isNumber(event);"
                                        autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <asp:HiddenField ID="hdnFullCarNumber" runat="server" />
                            <div class="car-number-validation">
                                <small class="text-muted">الصيغة: حرف + حرف + حرف (اختياري) + ١-٤ أرقام</small>
                            </div>
                        </div>
                        <div class="col-md-4 text-center">
                            <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-search w-100 mt-3" OnClick="btnSearch_Click" OnClientClick="return validateSearchForm();" />
                        </div>
                    </div>
                </div>

                <!-- النتائج -->
                <div class="result-box" id="resultBox" runat="server" visible="false">
                    <asp:HiddenField ID="hdnVehicleID" runat="server" Value="" />
                    <asp:HiddenField ID="hdnCarNumberId" runat="server" Value="" />

                    <!-- البيانات الأساسية -->
                    <div class="section-header">
                        <i class="fas fa-car"></i> البيانات الأساسية
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-4">
                                <div class="info-label">رقم السيارة</div>
                                <div class="info-value">
                                    <asp:Label ID="lblCarNumber" runat="server" Font-Bold="true" Font-Size="18px"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">نوع السيارة</label>
                                <asp:TextBox ID="txtType" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">الموديل</label>
                                <asp:TextBox ID="txtModel" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">سنة الصنع</label>
                                <asp:TextBox ID="txtYear" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">حالة السيارة</label>
                                <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>
                    </div>

                    <!-- البيانات الفنية -->
                    <div class="section-header">
                        <i class="fas fa-cogs"></i> البيانات الفنية
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label class="form-label">رقم الشاسيه</label>
                                <asp:TextBox ID="txtChassisView" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">رقم الموتور</label>
                                <asp:TextBox ID="txtMotorView" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">نوع الوقود</label>
                                <asp:TextBox ID="txtGasView" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">عدد الركاب</label>
                                <asp:TextBox ID="txtPassengersView" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">نوع الترخيص</label>
                                <asp:TextBox ID="txtLicenseTypeView" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>
                    </div>

                    <!-- الترخيص والعداد -->
                    <div class="row">
                        <div class="col-md-6">
                            <div class="section-header">
                                <i class="fas fa-id-card"></i> الترخيص
                            </div>
                            <div class="info-card">
                                <div class="row g-3">
                                    <div class="col-md-6">
                                        <label class="form-label">بداية الترخيص</label>
                                        <asp:TextBox ID="txtLicenseStartView" runat="server" CssClass="form-control" ReadOnly="true" />
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">نهاية الترخيص</label>
                                        <asp:TextBox ID="txtLicenseEndView" runat="server" CssClass="form-control" ReadOnly="true" />
                                    </div>
                                    <div class="col-md-12">
                                        <label class="form-label">وحدة المرور</label>
                                        <asp:TextBox ID="txtTrafficUnitView" runat="server" CssClass="form-control" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="warning-box" id="licenseWarning" runat="server" visible="false">
                                    <i class="fas fa-exclamation-triangle"></i> 
                                    <asp:Label ID="lblLicenseWarning" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="section-header">
                                <i class="fas fa-tachometer-alt"></i> العداد
                            </div>
                            <div class="km-display-box">
                                <div class="km-icon">
                                    <i class="fas fa-flag-checkered" style="color: #0d6efd; font-size: 32px;"></i>
                                </div>
                                <div class="km-label" style="color: #0d6efd; font-weight: bold;">
                                    آخر قراءة عداد
                                </div>
                                <div class="km-value" style="color: #0d6efd;">
                                    <asp:Label ID="lblLastEndKM" runat="server" Text="0"></asp:Label>
                                </div>
                                <div class="km-unit" style="color: #0d6efd;">كم</div>
                                <div class="km-date">
                                    <i class="fas fa-calendar-alt"></i>
                                    <asp:Label ID="lblLastKMDate" runat="server" Text="لا توجد قراءات"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- الملاحظات -->
                    <div class="section-header mt-3">
                        <i class="fas fa-sticky-note"></i> الملاحظات
                    </div>
                    <div class="info-card">
                        <asp:TextBox ID="txtNoteView" runat="server" CssClass="form-control" ReadOnly="true" TextMode="MultiLine" Rows="3" />
                    </div>

                    <!-- أزرار الأقسام الإضافية -->
                    <div class="section-buttons mt-4">
                        <button type="button" class="btn-section" onclick="showSection('insurance')">
                            <i class="fas fa-shield-alt"></i> التأمين <span class="badge-count" id="insuranceCount">0</span>
                        </button>
                        <button type="button" class="btn-section" onclick="showSection('maintenance')">
                            <i class="fas fa-wrench"></i> الصيانة <span class="badge-count" id="maintenanceCount">0</span>
                        </button>
                        <button type="button" class="btn-section" onclick="showSection('subscription')">
                            <i class="fas fa-users"></i> المشتركين <span class="badge-count" id="subscriptionCount">0</span>
                        </button>
                        <button type="button" class="btn-section" onclick="showSection('routes')">
                            <i class="fas fa-route"></i> المسارات <span class="badge-count" id="routesCount">0</span>
                        </button>
                    </div>

                    <!-- قسم التأمين -->
                    <div id="insuranceSection" class="section-container">
                        <div class="section-header">
                            <i class="fas fa-shield-alt"></i> سجل التأمين
                        </div>
                        <div class="info-card">
                            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                            <asp:UpdatePanel ID="upInsurance" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvInsurance" runat="server" 
                                        CssClass="table table-bordered table-striped" 
                                        AutoGenerateColumns="false"
                                        GridLines="None"
                                        EmptyDataText="لا توجد بيانات تأمين"
                                        Font-Size="14px"
                                        AllowPaging="true"
                                        PageSize="5"
                                        OnPageIndexChanging="gvInsurance_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="company_name" HeaderText="شركة التأمين" />
                                            <asp:BoundField DataField="insurance_name" HeaderText="نوع التأمين" />
                                            <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="cost" HeaderText="التكلفة" DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
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
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <!-- قسم الصيانة -->
                    <div id="maintenanceSection" class="section-container">
                        <div class="section-header">
                            <i class="fas fa-wrench"></i> سجل الصيانة
                        </div>
                        <div class="info-card">
                            <asp:UpdatePanel ID="upMaintenance" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvMaintenance" runat="server" 
                                        CssClass="table table-bordered table-striped" 
                                        AutoGenerateColumns="false"
                                        GridLines="None"
                                        EmptyDataText="لا توجد بيانات صيانة"
                                        Font-Size="14px"
                                        AllowPaging="true"
                                        PageSize="10"
                                        OnPageIndexChanging="gvMaintenance_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="MaintenanceDate" HeaderText="تاريخ الصيانة" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="MaintenanceType" HeaderText="نوع الصيانة" />
                                            <asp:BoundField DataField="OdometerReading" HeaderText="قراءة العداد" DataFormatString="{0:N0}" ItemStyle-CssClass="km-column" />
                                            <asp:BoundField DataField="TotalCost" HeaderText="التكلفة" DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
                                            <asp:BoundField DataField="NextDueKM" HeaderText="الصيانة القادمة (كم)" DataFormatString="{0:N0}" ItemStyle-CssClass="km-column" />
                                            <asp:BoundField DataField="NextDueDate" HeaderText="تاريخ الصيانة القادمة" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="Notes" HeaderText="ملاحظات" />
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <!-- قسم المشتركين -->
                    <div id="subscriptionSection" class="section-container">
                        <div class="section-header">
                            <i class="fas fa-users"></i> سجل المشتركين
                        </div>
                        <div class="info-card">
                            <asp:UpdatePanel ID="upSubscription" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvSubscription" runat="server" 
                                        CssClass="table table-bordered table-striped" 
                                        AutoGenerateColumns="false"
                                        GridLines="None"
                                        EmptyDataText="لا توجد بيانات مشتركين"
                                        Font-Size="14px"
                                        AllowPaging="true"
                                        PageSize="10"
                                        OnPageIndexChanging="gvSubscription_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="name" HeaderText="اسم المشترك" />
                                            <asp:BoundField DataField="phone" HeaderText="الهاتف" />
                                            <asp:BoundField DataField="department" HeaderText="القسم" />
                                            <asp:BoundField DataField="position" HeaderText="الوظيفة" />
                                            <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                                            <asp:BoundField DataField="cost" HeaderText="التكلفة" DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
                                            <asp:BoundField DataField="sub_count" HeaderText="عدد المشتركين" />
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <!-- قسم المسارات -->
                    <div id="routesSection" class="section-container">
                        <div class="section-header">
                            <i class="fas fa-route"></i> المسارات
                        </div>
                        <div class="info-card">
                            <asp:UpdatePanel ID="upRoutes" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvRoutes" runat="server" 
                                        CssClass="table table-bordered table-striped" 
                                        AutoGenerateColumns="false"
                                        GridLines="None"
                                        EmptyDataText="لا توجد بيانات مسارات"
                                        Font-Size="14px"
                                        AllowPaging="true"
                                        PageSize="10"
                                        OnPageIndexChanging="gvRoutes_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="route_name" HeaderText="اسم المسار" />
                                            <asp:BoundField DataField="description" HeaderText="الوصف" />
                                            <asp:BoundField DataField="start_loc" HeaderText="نقطة البداية" />
                                            <asp:BoundField DataField="end_loc" HeaderText="نقطة النهاية" />
                                            <asp:BoundField DataField="KM" HeaderText="المسافة (كم)" DataFormatString="{0:N0}" ItemStyle-CssClass="km-column" />
                                            <asp:BoundField DataField="direction" HeaderText="الاتجاه" />
                                            <asp:BoundField DataField="subscription_count" HeaderText="عدد المشتركين" />
                                            <asp:CheckBoxField DataField="is_active" HeaderText="نشط" />
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <script type="text/javascript">
        // Entry form functions
        function moveToNext(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) {
                    nextField.focus();
                }
            }
            current.classList.remove('error');
        }

        function moveToPrevious(current, prevFieldId) {
            if (current.value.length === 0 && prevFieldId) {
                var prevField = document.getElementById(prevFieldId);
                if (prevField) {
                    prevField.focus();
                }
            }
        }

        // Search form functions
        function moveToNextSearch(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) {
                    nextField.focus();
                }
            }
        }

        function moveToPreviousSearch(current, prevFieldId) {
            if (current.value.length === 0 && prevFieldId) {
                var prevField = document.getElementById(prevFieldId);
                if (prevField) {
                    prevField.focus();
                }
            }
        }

        function isArabicLetter(event) {
            var charCode = event.which || event.keyCode;
            var charStr = String.fromCharCode(charCode);
            var arabicPattern = /[\u0600-\u06FF]/;
            if (charCode === 8 || charCode === 9 || charCode === 13 ||
                charCode === 37 || charCode === 39 || charCode === 46) {
                return true;
            }
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

        // Validate chassis number (VIN)
        function validateChassisNumber(chassisNumber) {
            if (!chassisNumber || chassisNumber.trim() === '') {
                return { valid: false, message: 'رقم الشاسيه مطلوب' };
            }

            // Remove any spaces and convert to uppercase
            chassisNumber = chassisNumber.trim().toUpperCase();

            // Check length - must be exactly 17 characters
            if (chassisNumber.length !== 17) {
                return { valid: false, message: 'رقم الشاسيه يجب أن يكون 17 رمزاً بالضبط' };
            }

            // Check for forbidden letters I and O
            if (chassisNumber.includes('I') || chassisNumber.includes('O')) {
                return { valid: false, message: 'رقم الشاسيه لا يمكن أن يحتوي على الحرفين I أو O (لتجنب الخلط مع الأرقام 1 و 0)' };
            }

            // Check allowed characters: A-Z (except I,O) and 0-9
            var allowedPattern = /^[A-HJ-NPR-Z0-9]{17}$/;
            if (!allowedPattern.test(chassisNumber)) {
                return { valid: false, message: 'رقم الشاسيه يجب أن يحتوي فقط على أرقام (0-9) وحروف أبجدية (ما عدا I, O)' };
            }

            // Extract VIN components for information
            var countryCode = chassisNumber.substring(0, 1);
            var manufacturer = chassisNumber.substring(1, 3);
            var vehicleAttributes = chassisNumber.substring(3, 8);
            var checkDigit = chassisNumber.substring(8, 9);
            var modelYear = chassisNumber.substring(9, 10);
            var assemblyPlant = chassisNumber.substring(10, 11);
            var serialNumber = chassisNumber.substring(11, 17);

            // Year code mapping (10th character)
            var yearCodes = {
                'A': 1980, 'B': 1981, 'C': 1982, 'D': 1983, 'E': 1984, 'F': 1985, 'G': 1986, 'H': 1987, 'J': 1988, 'K': 1989, 'L': 1990,
                'M': 1991, 'N': 1992, 'P': 1993, 'R': 1994, 'S': 1995, 'T': 1996, 'V': 1997, 'W': 1998, 'X': 1999, 'Y': 2000,
                '1': 2001, '2': 2002, '3': 2003, '4': 2004, '5': 2005, '6': 2006, '7': 2007, '8': 2008, '9': 2009,
                'A': 2010, 'B': 2011, 'C': 2012, 'D': 2013, 'E': 2014, 'F': 2015, 'G': 2016, 'H': 2017, 'J': 2018, 'K': 2019, 'L': 2020,
                'M': 2021, 'N': 2022, 'P': 2023, 'R': 2024, 'S': 2025, 'T': 2026, 'V': 2027, 'W': 2028, 'X': 2029, 'Y': 2030
            };

            var decodedYear = yearCodes[modelYear];

            var infoMessage = '✅ رقم الشاسيه صحيح\n';
            infoMessage += '┌─────────────────────────────────┐\n';
            infoMessage += '│ مكونات رقم الشاسيه (VIN)        │\n';
            infoMessage += '├─────────────────────────────────┤\n';
            infoMessage += '│ بلد المنشأ: ' + countryCode + ' (' + getCountryName(countryCode) + ')' + '\n';
            infoMessage += '│ الشركة المصنعة: ' + manufacturer + '\n';
            infoMessage += '│ مواصفات السيارة: ' + vehicleAttributes + '\n';
            infoMessage += '│ رقم التحقق: ' + checkDigit + '\n';
            infoMessage += '│ سنة الصنع: ' + (decodedYear || 'غير معروف') + '\n';
            infoMessage += '│ مصنع التجميع: ' + assemblyPlant + '\n';
            infoMessage += '│ الرقم التسلسلي: ' + serialNumber + '\n';
            infoMessage += '└─────────────────────────────────┘';

            return { valid: true, message: infoMessage, info: infoMessage };
        }

        // Helper function to get country name from code
        function getCountryName(code) {
            var countries = {
                '1': 'الولايات المتحدة', '2': 'كندا', '3': 'المكسيك',
                'J': 'اليابان', 'K': 'كوريا', 'L': 'الصين', 'M': 'الهند',
                'S': 'المملكة المتحدة', 'T': 'سويسرا', 'V': 'فرنسا',
                'W': 'ألمانيا', 'Y': 'السويد', 'Z': 'إيطاليا',
                'A': 'جنوب أفريقيا', 'C': 'الصين', 'D': 'أستراليا'
            };
            return countries[code] || 'غير محدد';
        }

        // Validate license dates
        function validateLicenseDates() {
            var startDateInput = document.getElementById('<%= txtLicenseStart.ClientID %>');
            var endDateInput = document.getElementById('<%= txtLicenseEnd.ClientID %>');

            if (!startDateInput.value || !endDateInput.value) {
                return true; // Don't validate if empty, other validation will catch it
            }

            var startDate = new Date(startDateInput.value);
            var endDate = new Date(endDateInput.value);

            if (endDate <= startDate) {
                alert('❌ تاريخ نهاية الترخيص يجب أن يكون بعد تاريخ البداية');
                endDateInput.focus();
                return false;
            }

            // Optional: Check if license is expiring soon (within 30 days)
            var today = new Date();
            var daysUntilExpiry = Math.ceil((endDate - today) / (1000 * 60 * 60 * 24));

            if (daysUntilExpiry > 0 && daysUntilExpiry < 30) {
                var confirmMsg = '⚠️ تحذير: الترخيص سينتهي بعد ' + daysUntilExpiry + ' يوم.\nهل تريد المتابعة؟';
                return confirm(confirmMsg);
            }

            if (daysUntilExpiry <= 0) {
                var expiredMsg = '⚠️ تحذير: الترخيص منتهي الصلاحية!\nهل تريد المتابعة؟';
                return confirm(expiredMsg);
            }

            return true;
        }

        // Real-time chassis validation
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
                    messageDiv.innerHTML = '✅ ' + result.message.replace(/\n/g, '<br>');
                }
            } else {
                chassisInput.classList.remove('chassis-valid', 'chassis-invalid');
                messageDiv.className = 'validation-message';
                messageDiv.innerHTML = '';
            }
        }

        function validateEntryForm() {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtCarLetter2.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');

            if (!letter1.value || !letter2.value || !numbers.value) {
                alert('❌ الرجاء إدخال رقم السيارة كاملاً');
                return false;
            }

            var ddlCarType = document.getElementById('<%= ddlCarType.ClientID %>');
            if (ddlCarType && ddlCarType.value === '') {
                alert('❌ الرجاء اختيار نوع السيارة');
                ddlCarType.focus();
                return false;
            }

            var ddlCarModel = document.getElementById('<%= ddlCarModel.ClientID %>');
            if (ddlCarModel && ddlCarModel.value === '') {
                alert('❌ الرجاء اختيار الموديل');
                ddlCarModel.focus();
                return false;
            }

            var ddlYear = document.getElementById('<%= ddlYear.ClientID %>');
            if (ddlYear && ddlYear.value === '') {
                alert('❌ الرجاء اختيار سنة الصنع');
                ddlYear.focus();
                return false;
            }

            // Validate chassis number (رقم الشاسيه)
            var chassisInput = document.getElementById('<%= txtChassis.ClientID %>');
            if (chassisInput && chassisInput.value) {
                var chassisResult = validateChassisNumber(chassisInput.value);
                if (!chassisResult.valid) {
                    alert('❌ ' + chassisResult.message);
                    chassisInput.focus();
                    return false;
                } else {
                    // Show VIN info to user (optional)
                    if (!confirm(chassisResult.message + '\n\nهل تريد المتابعة بحفظ البيانات؟')) {
                        return false;
                    }
                }
            } else {
                alert('❌ الرجاء إدخال رقم الشاسيه');
                chassisInput.focus();
                return false;
            }

            // Validate license dates
            return validateLicenseDates();
        }

        function validateSearchForm() {
            var letter1 = document.getElementById('<%= txtSearchLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtSearchLetter2.ClientID %>');
            var numbers = document.getElementById('<%= txtSearchNumbers.ClientID %>');

            if (!letter1.value || !letter2.value || !numbers.value) {
                alert('❌ الرجاء إدخال رقم السيارة كاملاً');
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

            if (letter1 && letter2 && numbers && hiddenField) {
                var fullNumber = letter1.value + ' ' + letter2.value;
                if (letter3 && letter3.value) {
                    fullNumber += ' ' + letter3.value;
                }
                fullNumber += ' ' + numbers.value;
                hiddenField.value = fullNumber;
            }
        }

        function showSection(sectionName) {
            var sections = document.getElementsByClassName('section-container');
            for (var i = 0; i < sections.length; i++) {
                sections[i].classList.remove('active');
            }
            var buttons = document.getElementsByClassName('btn-section');
            for (var i = 0; i < buttons.length; i++) {
                buttons[i].classList.remove('active');
            }
            var selectedSection = document.getElementById(sectionName + 'Section');
            if (selectedSection) {
                selectedSection.classList.add('active');
            }
            if (event && event.target) {
                event.target.classList.add('active');
            }
        }

        document.addEventListener('DOMContentLoaded', function() {
            var numbersField = document.getElementById('<%= txtSearchNumbers.ClientID %>');
            if (numbersField) {
                numbersField.addEventListener('blur', updateFullCarNumber);
            }
            
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtCarLetter2.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');
            
            if (letter1) {
                letter1.addEventListener('input', function() { validateCarNumber(false); });
            }
            if (letter2) {
                letter2.addEventListener('input', function() { validateCarNumber(false); });
            }
            if (numbers) {
                numbers.addEventListener('input', function() { validateCarNumber(false); });
            }
            
            // Initialize chassis validation
            validateChassisRealTime();
        });

        function validateCarNumber(showAlert) {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtCarLetter2.ClientID %>');
            var letter3 = document.getElementById('<%= txtCarLetter3.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');

            if (!letter1 || !letter2 || !numbers) return false;

            letter1.classList.remove('error', 'valid');
            letter2.classList.remove('error', 'valid');
            if (letter3) letter3.classList.remove('error', 'valid');
            numbers.classList.remove('error', 'valid');

            var isValid = true;

            if (!letter1.value || letter1.value.trim() === '') {
                letter1.classList.add('error');
                isValid = false;
            } else {
                letter1.classList.add('valid');
            }

            if (!letter2.value || letter2.value.trim() === '') {
                letter2.classList.add('error');
                isValid = false;
            } else {
                letter2.classList.add('valid');
            }

            if (letter3 && letter3.value && letter3.value.trim() !== '') {
                letter3.classList.add('valid');
            }

            if (!numbers.value || numbers.value.trim() === '') {
                numbers.classList.add('error');
                isValid = false;
            } else {
                var numberStr = numbers.value.trim();
                var isValidNumber = true;
                for (var i = 0; i < numberStr.length; i++) {
                    var char = numberStr.charAt(i);
                    if (char >= '0' && char <= '9') continue;
                    if (char >= '٠' && char <= '٩') continue;
                    isValidNumber = false;
                    break;
                }
                if (numberStr.length < 1 || numberStr.length > 4) {
                    isValidNumber = false;
                }
                if (isValidNumber) {
                    numbers.classList.add('valid');
                } else {
                    numbers.classList.add('error');
                    isValid = false;
                }
            }
            return isValid;
        }
    </script>
</asp:Content>