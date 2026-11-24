<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
    CodeBehind="traffic_violations.aspx.cs" Inherits="car_WebApplication1.traffic_violations" 
    EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            color: #000;
            font-weight: bold;
            font-size: 28px;
            margin-bottom: 25px;
        }
        .search-box {
            border: 2px solid #000;
            border-radius: 10px;
            padding: 20px;
            background-color: #fff;
            margin-bottom: 25px;
        }
        .form-label {
            font-weight: bold;
            margin-bottom: 8px;
            color: #333;
            font-size: 14px;
        }
        .btn-search {
            background-color: #c8102e;
            color: #fff;
            font-weight: bold;
            height: 45px;
            font-size: 16px;
        }
        .btn-search:hover {
            background-color: #a50d25;
            color: #fff;
        }
        .btn-save {
            background: #a50d25;
            color: white;
            border: none;
            height: 45px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        .btn-save:hover {
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
        .result-box {
            border: 2px solid #000;
            border-radius: 10px;
            padding: 25px;
            margin-top: 20px;
            background-color: #f9f9f9;
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
        .input-group .btn-outline-secondary {
            border-color: #ced4da;
            height: 45px;
        }
        .input-group .btn-outline-secondary:hover {
            background-color: #dc3545;
            color: white;
            border-color: #dc3545;
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
        .action-btn {
            padding: 5px 10px;
            margin: 2px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 12px;
            display: inline-block;
            text-decoration: none;
        }
        .btn-edit {
            background-color: #ffc107;
            color: #000;
            border: none;
        }
        .btn-delete {
            background-color: #dc3545;
            color: white;
            border: none;
        }
        .btn-edit:hover, .btn-delete:hover {
            opacity: 0.8;
            transform: translateY(-1px);
        }
        .card {
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }
        .ms-2 {
            margin-right: 8px;
        }
        .grid-pager {
            background: #f8f9fa;
            padding: 15px;
            text-align: center;
            border-radius: 0 0 10px 10px;
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
        .badge-secondary {
            background-color: #6c757d;
            color: white;
            padding: 5px 10px;
            border-radius: 20px;
            font-size: 12px;
        }

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
        }
    </style>

    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css"/>
    <!-- jQuery -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <div class="container mt-4" dir="rtl">
        <!-- عنوان الصفحة -->
        <div class="text-center">
            <h2 class="page-title">
                <i class="fas fa-exclamation-triangle" style="color: #c8102e; margin-left: 10px;"></i>
                إدارة المخالفات المرورية
            </h2>
        </div>

        <!-- رسائل التنبيه -->
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <!-- Hidden field to remember active tab across postbacks -->
        <asp:HiddenField ID="hfActiveTab" runat="server" Value="tabSearchResults" />

        <!-- ===== TABS ===== -->
        <div class="nav-tabs-custom" id="mainTabs">
            <div class="nav-item">
                <button class="nav-link active" data-tab="tabSearchResults">
                    <i class="fas fa-search"></i> البحث والنتائج
                </button>
            </div>
            <div class="nav-item">
                <button class="nav-link" data-tab="tabAddViolation">
                    <i class="fas fa-plus-circle"></i> مخالفة جديدة
                </button>
            </div>
        </div>

        <div class="tab-content-custom">

            <!-- ===== TAB 1: SEARCH & RESULTS ===== -->
            <div class="tab-pane-custom active" id="tabSearchResults">
                <!-- قسم البحث عن المخالفات -->
                <div class="search-box">
                    <h5 class="fw-bold mb-3">
                        <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                        البحث عن المخالفات
                    </h5>
                    <div class="row g-3">
                        <div class="col-md-4">
                            <label class="form-label">رقم السيارة</label>
                            <asp:TextBox ID="txtSearchCar" runat="server" 
                                CssClass="form-control enhanced-dropdown" 
                                placeholder="أدخل رقم السيارة"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">من تاريخ</label>
                            <asp:TextBox ID="txtFromDate" runat="server" 
                                CssClass="form-control enhanced-dropdown" 
                                TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">إلى تاريخ</label>
                            <asp:TextBox ID="txtToDate" runat="server" 
                                CssClass="form-control enhanced-dropdown" 
                                TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-2 d-flex align-items-end">
                            <asp:Button ID="btnSearch" runat="server" 
                                Text="بحث" 
                                CssClass="btn btn-search w-100"
                                OnClick="btnSearch_Click" />
                        </div>
                    </div>
                    <div class="row g-3 mt-2">
                        <div class="col-md-2">
                            <asp:Button ID="btnReset" runat="server" 
                                Text="إعادة تعيين" 
                                CssClass="btn btn-secondary w-100"
                                OnClick="btnReset_Click" />
                        </div>
                    </div>
                </div>

                <!-- نتائج البحث -->
                <div id="searchResults" runat="server" class="result-box" visible="false">
                    <h5 class="fw-bold mb-3">
                        <i class="fas fa-list" style="color: #c8102e; margin-left: 8px;"></i>
                        نتائج البحث
                    </h5>
                    
                    <div class="table-responsive">
                        <asp:GridView ID="gvViolations" runat="server" 
                            CssClass="table table-striped table-bordered table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد نتائج للبحث"
                            AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvViolations_PageIndexChanging"
                            GridLines="None"
                            DataKeyNames="violation_id">
                            <Columns>
                                <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                                <asp:BoundField DataField="violation_id" HeaderText="رقم المخالفة" />
                                <asp:BoundField DataField="car_number" HeaderText="رقم السيارة" />
                                <asp:BoundField DataField="driver_name" HeaderText="اسم السائق" />
                                <asp:BoundField DataField="mission_date" HeaderText="تاريخ المهمة" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="violation_type" HeaderText="نوع المخالفة" />
                                <asp:BoundField DataField="violation_amount" HeaderText="القيمة" DataFormatString="{0:N2} ج.م" />
                                <asp:BoundField DataField="violation_date" HeaderText="تاريخ المخالفة" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="location" HeaderText="الموقع" />
                            </Columns>
                            <HeaderStyle CssClass="table-header" BackColor="#c8102e" ForeColor="White" />
                            <RowStyle CssClass="table-row" />
                            <AlternatingRowStyle CssClass="table-alt-row" />
                            <PagerStyle CssClass="grid-pager" />
                        </asp:GridView>
                    </div>
                </div>

                <!-- إحصائيات سريعة -->
                <div id="statsContainer" runat="server" visible="false" class="row mt-4">
                    <div class="col-md-6">
                        <div class="card bg-danger text-white">
                            <div class="card-body">
                                <h5 class="card-title">إجمالي المخالفات</h5>
                                <h2><asp:Label ID="lblTotalViolations" runat="server" Text="0"></asp:Label></h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="card bg-warning text-dark">
                            <div class="card-body">
                                <h5 class="card-title">إجمالي القيمة</h5>
                                <h2><asp:Label ID="lblTotalAmount" runat="server" Text="0"></asp:Label> ج.م</h2>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ===== TAB 2: ADD VIOLATION ===== -->
            <div class="tab-pane-custom" id="tabAddViolation">
                <div class="result-box mb-4">
                    <div class="section-header">
                        <i class="fas fa-plus-circle"></i> تسجيل مخالفة جديدة
                    </div>

                    <div class="row g-3">
                        <!-- نوع المخالفة -->
                        <div class="col-md-6">
                            <label class="form-label">نوع المخالفة <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlViolationType" runat="server"
                                CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="-- اختر نوع المخالفة --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <!-- رقم العربية - Dropdown (بدون سائق) -->
                        <div class="col-md-6">
                            <label class="form-label">رقم العربية <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlCarNumber" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlCarNumber_SelectedIndexChanged"
                                DataTextField="car_number_display"
                                DataValueField="car_number_id"
                                onchange="storeCurrentTab();">
                                <asp:ListItem Text="-- اختر رقم العربية --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row g-3 mt-3">
                        <!-- المهمة Dropdown -->
                        <div class="col-md-6">
                            <label class="form-label">المهمة <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlMission" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                DataTextField="mission_display"
                                DataValueField="mission_id">
                                <asp:ListItem Text="-- اختر المهمة --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <!-- قيمة المخالفة -->
                        <div class="col-md-2">
                            <label class="form-label">قيمة المخالفة <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtAmount" runat="server" 
                                CssClass="form-control enhanced-dropdown"
                                placeholder="المبلغ"
                                TextMode="Number" step="0.01" min="0"></asp:TextBox>
                        </div>
                      
                        <!-- تاريخ المخالفة -->
                        <div class="col-md-2">
                            <label class="form-label">تاريخ المخالفة <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtViolationDate" runat="server" 
                                CssClass="form-control enhanced-dropdown" 
                                TextMode="Date"></asp:TextBox>
                        </div>

                        <!-- الموقع -->
                        <div class="col-md-2">
                            <label class="form-label">الموقع</label>
                            <asp:TextBox ID="txtLocation" runat="server" 
                                CssClass="form-control enhanced-dropdown"
                                placeholder="مكان المخالفة"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row g-3 mt-3">
                        <!-- ملاحظات -->
                        <div class="col-md-8">
                            <label class="form-label">ملاحظات</label>
                            <asp:TextBox ID="txtNotes" runat="server" 
                                CssClass="form-control enhanced-dropdown"
                                placeholder="ملاحظات إضافية"
                                TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>

                        <!-- أزرار الإجراءات -->
                        <div class="col-md-4 d-flex align-items-end">
                            <asp:Button ID="btnSave" runat="server" Text="حفظ المخالفة" 
                                CssClass="btn btn-save w-100 me-2" OnClick="btnSave_Click" />
                            <asp:Button ID="btnClear" runat="server" Text="مسح" 
                                CssClass="btn btn-clear w-100" OnClick="btnClear_Click" />
                        </div>
                    </div>
                </div>
            </div>

        </div><!-- /.tab-content-custom -->

    </div>

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

        // Store current tab before any postback
        function storeCurrentTab() {
            var activeTabField = document.getElementById('<%= hfActiveTab.ClientID %>');
            if (activeTabField) {
                var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
                tabs.forEach(function (tab) {
                    if (tab.classList.contains('active')) {
                        activeTabField.value = tab.getAttribute('data-tab');
                    }
                });
            }
            return true;
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

            // Restore active tab after postback
            if (activeTabField && activeTabField.value) {
                activateTab(activeTabField.value);
            }

            // If search results are visible, switch to search tab
            var searchResults = document.getElementById('<%= searchResults.ClientID %>');
            if (searchResults && searchResults.style.display !== 'none') {
                activateTab('tabSearchResults');
            }

            // Store current tab before any postback via form submit
            var forms = document.getElementsByTagName('form');
            if (forms.length > 0) {
                forms[0].addEventListener('submit', function () {
                    storeCurrentTab();
                });
            }

            // Also store when dropdowns change (for AutoPostBack)
            var dropdowns = document.querySelectorAll('select[onchange*="storeCurrentTab"]');
            dropdowns.forEach(function (ddl) {
                ddl.addEventListener('change', function () {
                    storeCurrentTab();
                });
            });
        });
    </script>

</asp:Content>