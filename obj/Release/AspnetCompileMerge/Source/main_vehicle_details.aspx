<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="main_vehicle_details.aspx.cs"
    Inherits="car_WebApplication1.main_vehicle_details" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }

        .search-box {
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
            text-align: center;
        }

        .btn-search {
            background-color: #c8102e;
            color: #fff;
            font-weight: bold;
            height: 50px;
        }

        .btn-search:hover {
            background-color: #a50d25;
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
            background-color: #c8102e;
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
            background-color: #c8102e;
            color: white;
            border-radius: 50%;
            padding: 2px 8px;
            margin-right: 5px;
            font-size: 12px;
        }
        
        @media (max-width: 768px) {
            .car-letter-box {
                width: 50px;
            }
            
            .car-number-single-box {
                width: 80px;
            }
            
            .km-value {
                font-size: 32px;
            }
            
            .section-buttons {
                flex-direction: column;
            }
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
        
        .empty-data {
            text-align: center;
            color: #6c757d;
            padding: 20px;
            font-style: italic;
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
            color: #c8102e;
            font-weight: bold;
        }

        .km-column {
            color: #0d6efd;
            font-weight: bold;
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <!-- العنوان -->
        <div class="text-center mb-4">
            <h2 class="page-title">استعلام عن بيانات سيارة</h2>
        </div>

        <!-- رسالة الخطأ -->
        <asp:Label ID="lblMessage"
            runat="server"
            CssClass="alert alert-danger text-center d-block"
            Visible="false">
        </asp:Label>
        
        <!-- رسالة النجاح -->
        <asp:Label ID="lblSuccess"
            runat="server"
            CssClass="alert alert-success text-center d-block"
            Visible="false">
        </asp:Label>

        <!-- البحث -->
        <div class="search-box mb-4">
            <div class="row align-items-end">
                <div class="col-md-8">
                    <label class="form-label">رقم السيارة</label>
                    
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
                                onkeyup="updateFullCarNumber();"
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter3'); }"
                                onkeypress="return isNumber(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <asp:HiddenField ID="hdnFullCarNumber" runat="server" />
                    
                    <div class="car-number-validation" id="carNumberValidation">
                        <small class="text-muted">الصيغة: حرف + حرف + حرف (اختياري) + ١-٤ أرقام</small>
                    </div>
                </div>
                
                <div class="col-md-4 text-center">
                    <asp:Button ID="btnSearch"
                        runat="server"
                        Text="بحث"
                        CssClass="btn btn-search w-100 mt-3"
                        OnClick="btnSearch_Click"
                        OnClientClick="return validateSearchForm();" />
                </div>
            </div>
        </div>

        <!-- النتائج -->
        <div class="result-box" id="resultBox" runat="server" visible="false">

            <!-- Vehicle ID (hidden) -->
            <asp:HiddenField ID="hdnVehicleID" runat="server" Value="" />
            <asp:HiddenField ID="hdnCarNumberId" runat="server" Value="" />

            <!-- ========== القسم الأساسي (يظهر دائماً) ========== -->
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
                        <asp:TextBox ID="txtCarNum" runat="server" CssClass="form-control" ReadOnly="true" Visible="false" />
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

            <!-- ========== القسم الفني (يظهر دائماً) ========== -->
            <div class="section-header">
                <i class="fas fa-cogs"></i> البيانات الفنية
            </div>
            
            <div class="info-card">
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label">رقم الشاسيه</label>
                        <asp:TextBox ID="txtChassis" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">رقم الموتور</label>
                        <asp:TextBox ID="txtMotor" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">نوع الوقود</label>
                        <asp:TextBox ID="txtGas" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">عدد الركاب</label>
                        <asp:TextBox ID="txtPassengers" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">نوع الترخيص</label>
                        <asp:TextBox ID="txtLicenseType" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>

            <!-- ========== الترخيص والعداد (يظهر دائماً) ========== -->
            <div class="row">
                <div class="col-md-6">
                    <div class="section-header">
                        <i class="fas fa-id-card"></i> الترخيص
                    </div>
                    <div class="info-card">
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label class="form-label">بداية الترخيص</label>
                                <asp:TextBox ID="txtLicenseStart" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">نهاية الترخيص</label>
                                <asp:TextBox ID="txtLicenseEnd" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-12">
                                <label class="form-label">وحدة المرور</label>
                                <asp:TextBox ID="txtTrafficUnit" runat="server" CssClass="form-control" ReadOnly="true" />
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

            <!-- ========== معلومات إضافية من main_vehicle_details ========== -->
<div class="row mt-3">
    <div class="col-md-6">
        <div class="info-card">
            <label class="form-label">مكان الانتظار</label>
            <asp:TextBox ID="txtParking" runat="server" CssClass="form-control" ReadOnly="true" />
        </div>
    </div>
    <div class="col-md-6">
        <div class="info-card">
            <label class="form-label">تكلفة الانتظار</label>
            <asp:TextBox ID="txtParkingCost" runat="server" CssClass="form-control" ReadOnly="true" />
        </div>
    </div>
</div>

            <!-- ========== أزرار الأقسام الإضافية ========== -->
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

            <!-- ========== قسم التأمين ========== -->
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
                                    <asp:BoundField DataField="provider" HeaderText="مقدم الخدمة" />
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

<!-- ========== قسم الصيانة ========== -->
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

            <!-- ========== قسم المشتركين ========== -->
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
                                    <asp:BoundField DataField="position" HeaderText="المنصب" />
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

            <!-- ========== قسم المسارات ========== -->
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

            <!-- ========== الملاحظات ========== -->
            <div class="section-header mt-3">
                <i class="fas fa-sticky-note"></i> الملاحظات
            </div>
            <div class="info-card">
                <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" ReadOnly="true" TextMode="MultiLine" Rows="3" />
            </div>

        </div>

    </div>

    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <!-- JavaScript -->
    <script type="text/javascript">
        function moveToNext(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) {
                    nextField.focus();
                }
            }
        }

        function moveToPrevious(current, prevFieldId) {
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

        function updateFullCarNumber() {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtCarLetter2.ClientID %>');
            var letter3 = document.getElementById('<%= txtCarLetter3.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');
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

        function validateCarNumber() {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtCarLetter2.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');
            
            if (!letter1 || !letter2 || !numbers) return false;
            
            letter1.classList.remove('error', 'valid');
            letter2.classList.remove('error', 'valid');
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
            
            if (!numbers.value || numbers.value.trim() === '') {
                numbers.classList.add('error');
                isValid = false;
            } else {
                var numberPattern = /^[0-9]{1,4}$/;
                if (numberPattern.test(numbers.value.trim())) {
                    numbers.classList.add('valid');
                } else {
                    numbers.classList.add('error');
                    isValid = false;
                }
            }
            
            return isValid;
        }

        function validateSearchForm() {
            if (!validateCarNumber()) {
                alert('الرجاء إدخال رقم سيارة صحيح (حرف + حرف + حرف اختياري + 1-4 أرقام)');
                return false;
            }
            
            updateFullCarNumber();
            return true;
        }

        // Show/Hide Sections
        function showSection(sectionName) {
            // Hide all sections
            var sections = document.getElementsByClassName('section-container');
            for (var i = 0; i < sections.length; i++) {
                sections[i].classList.remove('active');
            }
            
            // Remove active class from all buttons
            var buttons = document.getElementsByClassName('btn-section');
            for (var i = 0; i < buttons.length; i++) {
                buttons[i].classList.remove('active');
            }
            
            // Show selected section
            var selectedSection = document.getElementById(sectionName + 'Section');
            if (selectedSection) {
                selectedSection.classList.add('active');
            }
            
            // Add active class to clicked button
            event.target.classList.add('active');
        }

        document.addEventListener('DOMContentLoaded', function() {
            var numbersField = document.getElementById('<%= txtCarNumbers.ClientID %>');
            if (numbersField) {
                numbersField.addEventListener('blur', updateFullCarNumber);
            }
        });
    </script>
</asp:Content>