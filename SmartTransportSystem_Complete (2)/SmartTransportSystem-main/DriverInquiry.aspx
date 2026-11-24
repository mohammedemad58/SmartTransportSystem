<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="DriverInquiry.aspx.cs"
    Inherits="car_WebApplication1.DriverInquiry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            font-size: 28px;
            margin-bottom: 25px;
        }

        .search-box {
            border: 2px solid #000;
            border-radius: 14px;
            padding: 25px;
            background-color: #fff;
            margin-bottom: 25px;
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
            margin-bottom: 8px;
            color: #333;
            font-size: 14px;
        }

        .form-control {
            text-align: right;
        }

        .btn-search {
            background-color: #c8102e;
            color: #fff;
            font-weight: bold;
            height: 50px;
            font-size: 16px;
            border-radius: 8px;
            border: none;
            transition: all 0.3s ease;
        }

        .btn-search:hover {
            background-color: #a50d25;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }

        .btn-save {
            background: #c8102e;
            color: white;
            border: none;
            height: 45px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }

        .btn-save:hover {
            background: #a50d25;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }

        .btn-clear {
            background: #6c757d;
            color: white;
            border: none;
            height: 45px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }

        .btn-clear:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }

        .btn-warning {
            background: #ffc107;
            color: #2c3e50;
            border: none;
            padding: 5px 10px;
            border-radius: 4px;
            margin-left: 5px;
        }

        .btn-warning:hover {
            background: #e0a800;
        }

        .btn-success {
            background: #28a745;
            color: white;
            border: none;
            padding: 5px 10px;
            border-radius: 4px;
        }

        .btn-success:hover {
            background: #1e7e34;
        }

        .me-1 {
            margin-left: 5px;
        }

        .me-2 {
            margin-right: 8px;
        }

        .mt-2 {
            margin-top: 10px;
        }

        .mt-3 {
            margin-top: 15px;
        }

        .mb-4 {
            margin-bottom: 20px;
        }

        .enhanced-dropdown {
            background: #fff;
            border: 2px solid #dee2e6;
            border-radius: 8px;
            transition: all 0.3s ease;
            height: 45px;
            width: 100%;
            padding: 0 10px;
        }

        .enhanced-dropdown:hover {
            border-color: #c8102e;
        }

        .enhanced-dropdown:focus {
            border-color: #c8102e;
            box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1);
            outline: none;
        }

        .section-header {
            background: linear-gradient(90deg, #c8102e, #ffffff);
            padding: 10px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-weight: bold;
            color: #000;
            font-size: 18px;
        }

        .section-header i {
            margin-left: 10px;
            color: #c8102e;
        }

        .table-header {
            background-color: #c8102e;
            color: white;
        }

        .table-row:hover {
            background-color: #f5f5f5;
        }

        .table-alt-row {
            background-color: #f9f9f9;
        }

        .grid-pager {
            background: #f8f9fa;
            padding: 15px;
            text-align: center;
            border-radius: 0 0 10px 10px;
            margin-top: 10px;
        }

        .grid-pager table {
            margin: 0 auto;
        }

        .grid-pager td {
            padding: 5px 10px;
            border: none;
        }

        .grid-pager a {
            color: #c8102e;
            text-decoration: none;
            padding: 5px 10px;
            border-radius: 5px;
        }

        .grid-pager a:hover {
            background: #c8102e;
            color: white;
        }

        .grid-pager span {
            background: #c8102e;
            color: white;
            padding: 5px 10px;
            border-radius: 5px;
        }

        .stat-card {
            background: white;
            border-radius: 10px;
            padding: 20px;
            text-align: center;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            transition: all 0.3s ease;
        }

        .stat-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 12px rgba(0,0,0,0.15);
        }

        .stat-card.bg-danger {
            background: linear-gradient(135deg, #c8102e 0%, #a50d25 100%) !important;
            color: white;
        }

        .stat-card h3 {
            font-size: 32px;
            font-weight: 700;
            margin: 10px 0;
        }

        .stat-card p {
            margin: 0;
            font-size: 14px;
            opacity: 0.9;
        }

        .custom-input-group {
            display: flex;
            gap: 10px;
            align-items: center;
        }

        .custom-input-group .dropdown {
            flex: 2;
        }

        .custom-input-group .textbox {
            flex: 1;
        }

        .other-unit-container {
            margin-top: 10px;
        }

        /* ===== License expiry alert ===== */
        .license-alert {
            background: #fff3cd;
            border: 2px solid #ffc107;
            border-right: 8px solid #dc3545;
            border-radius: 10px;
            padding: 15px 20px;
            margin-bottom: 20px;
        }
        .license-alert-title { font-weight: bold; font-size: 16px; color: #721c24; margin-bottom: 8px; }
        .license-alert ul { margin: 0; padding-right: 20px; }
        .license-alert li { margin-bottom: 4px; }
        .license-expired { color: #dc3545; font-weight: bold; }
        .license-soon { color: #856404; }

        .lic-badge { display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: bold; color: white; white-space: nowrap; }
        .lic-ok { background-color: #28a745; }
        .lic-soon { background-color: #ffc107; color: #333; }
        .lic-expired { background-color: #dc3545; }
        .lic-none { background-color: #6c757d; }

        /* ===== Tab Navigation ===== */
        .nav-tabs-custom {
            border-bottom: 2px solid #dee2e6;
            margin-bottom: 20px;
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
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
            color: #c8102e;
            background: #f8f9fa;
        }

        .nav-tabs-custom .nav-link.active {
            color: #c8102e;
            border-bottom: 3px solid #c8102e;
            background: transparent;
        }

        .tab-content-custom {
            padding: 5px 0;
        }

        .tab-pane-custom {
            display: none;
            animation: fadeInTab 0.4s;
        }

        .tab-pane-custom.active {
            display: block;
        }

        @keyframes fadeInTab {
            from { opacity: 0; transform: translateY(10px); }
            to { opacity: 1; transform: translateY(0); }
        }

        @media (max-width: 768px) {
            .nav-tabs-custom .nav-link {
                font-size: 14px !important;
                padding: 10px 15px !important;
            }
            .stat-card h3 {
                font-size: 24px;
            }
            .custom-input-group {
                flex-direction: column;
            }
            .custom-input-group .dropdown,
            .custom-input-group .textbox {
                width: 100%;
            }
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <!-- العنوان -->
        <div class="text-center mb-4">
            <h2 class="page-title">
                <i class="fas fa-truck" style="color: #c8102e; margin-left: 10px;"></i>
                إدارة السائقين
            </h2>
        </div>

        <!-- رسائل -->
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <!-- License expiry alert: drivers whose license ends within 30 days or has already ended -->
        <asp:Panel ID="licenseAlertPanel" runat="server" CssClass="license-alert" Visible="false">
            <asp:Label ID="lblLicenseAlert" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Hidden field: the active tab. The server sets it (SetTab) and the script below restores it after every postback -->
        <asp:HiddenField ID="hfActiveTab" runat="server" Value="tabAddDriver" />

        <!-- ===== TABS (type="button" so they never submit the form) ===== -->
        <div class="nav-tabs-custom" id="mainTabs">
            <div class="nav-item">
                <button type="button" class="nav-link active" data-tab="tabAddDriver">
                    <i class="fas fa-user-plus"></i> سائق جديد
                </button>
            </div>
            <div class="nav-item">
                <button type="button" class="nav-link" data-tab="tabDriverList">
                    <i class="fas fa-list"></i> قائمة السائقين
                </button>
            </div>
            <div class="nav-item">
                <button type="button" class="nav-link" data-tab="tabSearchResults">
                    <i class="fas fa-search"></i> البحث والنتائج
                </button>
            </div>
        </div>

        <div class="tab-content-custom">

            <!-- ===== TAB 1: ADD DRIVER ===== -->
            <div class="tab-pane-custom active" id="tabAddDriver">
                <div class="result-box mb-4" id="driverFormSection">
                    <div class="section-header">
                        <i class="fas fa-user-plus"></i> إضافة / تعديل سائق
                    </div>

                    <div class="row g-3">
                        <div class="col-md-4">
                            <label class="form-label">اسم السائق <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtDriverName" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل اسم السائق"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">الرقم القومي <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtNationalId" runat="server" CssClass="form-control enhanced-dropdown" placeholder="الرقم القومي (14 رقم)" MaxLength="14"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">رقم الهاتف</label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control enhanced-dropdown" placeholder="رقم الهاتف (11 رقم)" MaxLength="11"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row g-3 mt-2">
                        <div class="col-md-4">
                            <label class="form-label">رقم الرخصة <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtLicenseNumber" runat="server" CssClass="form-control enhanced-dropdown" placeholder="رقم الرخصة (14 رقم)" MaxLength="14"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">نوع الرخصة <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlLicenseType" runat="server" CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="-- اختر نوع الرخصة --" Value=""></asp:ListItem>
                                <asp:ListItem Text="درجة أولى" Value="درجة أولى"></asp:ListItem>
                                <asp:ListItem Text="درجة ثانية" Value="درجة ثانية"></asp:ListItem>
                                <asp:ListItem Text="درجة ثالثة" Value="درجة ثالثة"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">مبلغ تجديد الرخصة (جنيه)</label>
                            <asp:TextBox ID="txtRenewalAmount" runat="server" CssClass="form-control enhanced-dropdown" placeholder="المبلغ المدفوع للتجديد" TextMode="Number" step="0.01" min="0"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row g-3 mt-2">
                        <div class="col-md-6">
                            <label class="form-label">وحدة المرور</label>
                            <asp:DropDownList ID="ddlTrafficUnit" runat="server" CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="-- اختر وحدة المرور --" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">تاريخ بداية الرخصة <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtLicenseStartDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row g-3 mt-2">
                        <div class="col-md-6">
                            <label class="form-label">تاريخ نهاية الرخصة <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtLicenseEndDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-6 d-flex align-items-end">
                            <asp:Button ID="btnAddDriver" runat="server" Text="إضافة سائق" CssClass="btn btn-save w-100 me-2" OnClick="btnAddDriver_Click" />
                            <asp:Button ID="btnUpdateDriver" runat="server" Text="تحديث سائق" CssClass="btn btn-save w-100 me-2" Visible="false" OnClick="btnUpdateDriver_Click" />
                            <asp:Button ID="btnCancelDriver" runat="server" Text="إلغاء" CssClass="btn btn-clear w-100" Visible="false" OnClick="btnCancelDriver_Click" />
                        </div>
                    </div>
                </div>

                <!-- إحصائيات سريعة -->
                <div class="row mb-4">
                    <div class="col-md-12">
                        <div class="stat-card bg-danger">
                            <i class="fas fa-users fa-2x"></i>
                            <h3><asp:Label ID="lblTotalDrivers" runat="server" Text="0"></asp:Label></h3>
                            <p>إجمالي السائقين</p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ===== TAB 2: DRIVER LIST ===== -->
            <div class="tab-pane-custom" id="tabDriverList">
                <div class="result-box mb-4">
                    <div class="section-header">
                        <i class="fas fa-list"></i> قائمة السائقين
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="gvDriversList" runat="server"
                            CssClass="table table-striped table-bordered table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا يوجد سائقين"
                            OnRowCommand="gvDriversList_RowCommand"
                            AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvDriversList_PageIndexChanging"
                            GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="RowNum" HeaderText="#" />
                                <asp:BoundField DataField="driver_name" HeaderText="اسم السائق" />
                                <asp:BoundField DataField="license_number" HeaderText="رقم الرخصة" />
                                <asp:BoundField DataField="phone" HeaderText="رقم الهاتف" />
                                <asp:BoundField DataField="license_type" HeaderText="نوع الرخصة" />
                                <asp:BoundField DataField="traffic_unit" HeaderText="وحدة المرور" />
                                <asp:BoundField DataField="renewal_amount" HeaderText="مبلغ التجديد" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="license_start_date" HeaderText="تاريخ بداية الرخصة" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="license_end_date" HeaderText="تاريخ نهاية الرخصة" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:TemplateField HeaderText="حالة الرخصة">
                                    <ItemTemplate>
                                        <span class='<%# GetLicenseClass(Eval("license_state")) %>'><%# Eval("license_status") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="national_id" HeaderText="الرقم القومي" />
                                <asp:TemplateField HeaderText="الإجراءات">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server"
                                            CommandName="EditDriver"
                                            CommandArgument='<%# Eval("driver_id") %>'
                                            CssClass="btn btn-warning me-1"
                                            ToolTip="تعديل">
                                            <i class="fas fa-edit"></i> تعديل
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="table-header" BackColor="#c8102e" ForeColor="White" />
                            <RowStyle CssClass="table-row" />
                            <AlternatingRowStyle CssClass="table-alt-row" />
                            <PagerStyle CssClass="grid-pager" />
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <!-- ===== TAB 3: SEARCH & RESULTS ===== -->
            <div class="tab-pane-custom" id="tabSearchResults">
                <!-- البحث الموحد -->
                <div class="search-box">
                    <h5 class="fw-bold mb-3">
                        <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                        بحث عن سائق
                    </h5>

                    <div class="row g-3">
                        <div class="col-md-5">
                            <label class="form-label">البحث بالاسم</label>
                            <asp:TextBox ID="txtDriverSearch" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                placeholder="أدخل اسم السائق للبحث...">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-5">
                            <label class="form-label">وحدة المرور</label>
                            <asp:DropDownList ID="ddlSearchTrafficUnit" runat="server" CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="-- كل وحدات المرور --" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2 d-flex align-items-end">
                            <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-search w-100" OnClick="btnSearch_Click" />
                        </div>
                    </div>

                    <!-- نتائج البحث -->
                    <div id="driverSearchResults" runat="server" class="result-box mt-3" visible="false">
                        <h5 class="fw-bold mb-3">
                            <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                            نتائج البحث
                        </h5>
                        <div class="table-responsive">
                            <asp:GridView ID="gvDriverSearchResults" runat="server"
                                CssClass="table table-striped table-bordered table-hover"
                                AutoGenerateColumns="false"
                                EmptyDataText="لا توجد نتائج للبحث"
                                OnRowCommand="gvDriverSearchResults_RowCommand"
                                GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="RowNum" HeaderText="#" />
                                    <asp:BoundField DataField="driver_name" HeaderText="اسم السائق" />
                                    <asp:BoundField DataField="phone" HeaderText="الهاتف" />
                                    <asp:BoundField DataField="national_id" HeaderText="الرقم القومي" />
                                    <asp:BoundField DataField="traffic_unit" HeaderText="وحدة المرور" />
                                    <asp:BoundField DataField="renewal_amount" HeaderText="مبلغ التجديد" DataFormatString="{0:N2}" />
                                    <asp:TemplateField HeaderText="اختيار">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnSelectDriver" runat="server"
                                                CommandName="SelectDriver"
                                                CommandArgument='<%# Eval("driver_id") %>'
                                                CssClass="btn btn-success">
                                                <i class="fas fa-check"></i> اختيار
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="table-header" BackColor="#c8102e" ForeColor="White" />
                                <RowStyle CssClass="table-row" />
                                <AlternatingRowStyle CssClass="table-alt-row" />
                                <PagerStyle CssClass="grid-pager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <!-- Driver Statistics -->
                <div id="resultBox" runat="server" class="result-box" visible="false">
                    <div class="section-header">
                        <i class="fas fa-chart-line"></i> إحصائيات السائق
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="gvDriverTasks" runat="server"
                            CssClass="table table-striped table-bordered table-hover"
                            AutoGenerateColumns="true"
                            EmptyDataText="لا توجد بيانات"
                            AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvDriverTasks_PageIndexChanging"
                            GridLines="None">
                            <HeaderStyle CssClass="table-header" BackColor="#c8102e" ForeColor="White" />
                            <RowStyle CssClass="table-row" />
                            <AlternatingRowStyle CssClass="table-alt-row" />
                            <PagerStyle CssClass="grid-pager" />
                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div><!-- /.tab-content-custom -->

        <!-- Hidden fields for IDs -->
        <asp:HiddenField ID="hdnDriverId" runat="server" Value="0" />
        <asp:HiddenField ID="hdnEmployeeId" runat="server" Value="0" />

    </div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <script type="text/javascript">
        // ===== Tab Switching =====
        function activateTab(tabId) {
            var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
            var panes = document.querySelectorAll('.tab-pane-custom');

            tabs.forEach(function (t) { t.classList.remove('active'); });
            panes.forEach(function (p) { p.classList.remove('active'); });

            tabs.forEach(function (tab) {
                if (tab.getAttribute('data-tab') === tabId) {
                    tab.classList.add('active');
                }
            });

            var targetPane = document.getElementById(tabId);
            if (targetPane) targetPane.classList.add('active');

            var activeTabField = document.getElementById('<%= hfActiveTab.ClientID %>');
            if (activeTabField) activeTabField.value = tabId;
        }

        document.addEventListener('DOMContentLoaded', function () {
            var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
            var activeTabField = document.getElementById('<%= hfActiveTab.ClientID %>');

            // Click handlers
            tabs.forEach(function (tab) {
                tab.addEventListener('click', function (e) {
                    e.preventDefault();
                    activateTab(this.getAttribute('data-tab'));
                });
            });

            // Restore the active tab after a postback.
            // The server decides it (SetTab in the code-behind); the user's clicks update it too.
            if (activeTabField && activeTabField.value) {
                activateTab(activeTabField.value);
            }
        });

        // Allow only numbers
        function isNumber(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }

        // Enforce only numbers and max length
        function enforceNumberAndMaxLength(input, maxLength) {
            input.value = input.value.replace(/[^\d]/g, '');
            if (input.value.length > maxLength) {
                input.value = input.value.substring(0, maxLength);
            }
        }

        // Initialize all validations when page loads
        document.addEventListener('DOMContentLoaded', function () {
            // Phone input - 11 digits
            var phoneInput = document.getElementById('<%= txtPhone.ClientID %>');
            if (phoneInput) {
                phoneInput.addEventListener('input', function () {
                    enforceNumberAndMaxLength(this, 11);
                });
            }

            // National ID input - 14 digits
            var nationalIdInput = document.getElementById('<%= txtNationalId.ClientID %>');
            if (nationalIdInput) {
                nationalIdInput.addEventListener('input', function () {
                    enforceNumberAndMaxLength(this, 14);
                });
            }

            // License number input - 14 digits
            var licenseInput = document.getElementById('<%= txtLicenseNumber.ClientID %>');
            if (licenseInput) {
                licenseInput.addEventListener('input', function () {
                    enforceNumberAndMaxLength(this, 14);
                });
            }
        });
    </script>

</asp:Content>
