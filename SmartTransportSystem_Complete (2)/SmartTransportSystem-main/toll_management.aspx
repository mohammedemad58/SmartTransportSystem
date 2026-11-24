<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
    CodeBehind="toll_management.aspx.cs" Inherits="car_WebApplication1.toll_management" 
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
        .enhanced-dropdown {
            background: #fff;
            border: 2px solid #dee2e6;
            border-radius: 8px;
            transition: all 0.3s ease;
            height: 45px;
            width: 100%;
            padding: 0 10px;
            font-size: 14px;
        }
        .enhanced-dropdown:hover {
            border-color: #c8102e;
        }
        .enhanced-dropdown:focus {
            border-color: #c8102e;
            box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1);
            outline: none;
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
        .badge-subscribed {
            background-color: #28a745;
            color: white;
            padding: 5px 10px;
            border-radius: 20px;
            font-size: 12px;
        }
        .badge-pay-per-use {
            background-color: #ffc107;
            color: #000;
            padding: 5px 10px;
            border-radius: 20px;
            font-size: 12px;
        }
        .badge-secondary {
            background-color: #6c757d;
            color: white;
            padding: 5px 10px;
            border-radius: 20px;
            font-size: 12px;
        }
        .card {
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }
        .ms-2 {
            margin-right: 8px;
        }
        .mt-3 {
            margin-top: 15px;
        }
        
        /* Fix for dropdown text visibility */
        .enhanced-dropdown {
            background: #fff;
            border: 2px solid #dee2e6;
            border-radius: 8px;
            transition: all 0.3s ease;
            height: 45px;
            width: 100%;
            padding: 0 10px;
            font-size: 14px;
            color: #333 !important;
            background-color: white !important;
        }

        .enhanced-dropdown option {
            color: #333 !important;
            background: white !important;
            padding: 10px;
        }

        .enhanced-dropdown:focus,
        .enhanced-dropdown:active,
        .enhanced-dropdown:visited {
            color: #333 !important;
            background-color: white !important;
        }

        select.enhanced-dropdown {
            -webkit-appearance: menulist;
            -moz-appearance: menulist;
            appearance: menulist;
            background-image: url("data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23333' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='6 9 12 15 18 9'/></svg>");
            background-repeat: no-repeat;
            background-position: left 10px center;
            background-size: 16px;
            padding-left: 35px;
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
                <i class="fas fa-toll" style="color: #c8102e; margin-left: 10px;"></i>
                إدارة تحصيل الرسوم
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
                <button class="nav-link" data-tab="tabSubscription">
                    <i class="fas fa-ticket-alt"></i> اشتراك جديد
                </button>
            </div>
            <div class="nav-item">
                <button class="nav-link" data-tab="tabGates">
                    <i class="fas fa-road"></i> إدارة البوابات
                </button>
            </div>
        </div>

        <div class="tab-content-custom">

            <!-- ===== TAB 1: SEARCH & RESULTS ===== -->
            <div class="tab-pane-custom active" id="tabSearchResults">
                <!-- قسم البحث -->
                <div class="search-box">
                    <h5 class="fw-bold mb-3">
                        <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                        البحث عن المعاملات
                    </h5>
                    <div class="row g-3">
                        <div class="col-md-4">
                            <label class="form-label">رقم السيارة</label>
                            <asp:TextBox ID="txtSearchCar" runat="server" 
                                CssClass="form-control enhanced-dropdown" 
                                placeholder="أدخل رقم السيارة"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">البوابة</label>
                            <asp:DropDownList ID="ddlSearchGate" runat="server" CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="-- الكل --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">نوع الدفع</label>
                            <asp:DropDownList ID="ddlSearchType" runat="server"
                                CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="-- الكل --" Value="0"></asp:ListItem>
                                <asp:ListItem Text="اشتراك" Value="subscription"></asp:ListItem>
                                <asp:ListItem Text="دفع نقدي" Value="payment"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2 d-flex align-items-end">
                            <asp:Button ID="btnSearch" runat="server" 
                                Text="بحث" 
                                CssClass="btn btn-search w-100"
                                OnClick="btnSearch_Click" />
                        </div>
                    </div>
                    
                    <div class="row g-3 mt-3">
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
                        <div class="col-md-3">
                            <label class="form-label">تاريخ الاشتراك</label>
                            <asp:TextBox ID="txtSearchSubDate" runat="server" 
                                CssClass="form-control enhanced-dropdown" 
                                TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-3 d-flex align-items-end">
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
                        <asp:GridView ID="gvTransactions" runat="server" 
                            CssClass="table table-striped table-bordered table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد نتائج للبحث"
                            AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvTransactions_PageIndexChanging"
                            GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                                <asp:BoundField DataField="transaction_id" HeaderText="رقم المعاملة" />
                                <asp:BoundField DataField="car_number" HeaderText="رقم السيارة" />
                                <asp:BoundField DataField="gate_name" HeaderText="البوابة" />
                                <asp:BoundField DataField="amount" HeaderText="المبلغ" DataFormatString="{0:N2} ج.م" />
                                <asp:BoundField DataField="transaction_date" HeaderText="التاريخ" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:TemplateField HeaderText="نوع المعاملة">
                                    <ItemTemplate>
                                        <asp:Label ID="lblType" runat="server" 
                                            Text='<%# Eval("transaction_type").ToString() == "subscription" ? "اشتراك" : "دفع نقدي" %>'
                                            CssClass='<%# Eval("transaction_type").ToString() == "subscription" ? "badge-subscribed" : "badge-pay-per-use" %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="subscription_period" HeaderText="فترة الاشتراك" />
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
                    <div class="col-md-4">
                        <div class="card bg-danger text-white">
                            <div class="card-body">
                                <h5 class="card-title">إجمالي المعاملات</h5>
                                <h2><asp:Label ID="lblTotalTransactions" runat="server" Text="0"></asp:Label></h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card bg-warning text-dark">
                            <div class="card-body">
                                <h5 class="card-title">إجمالي المبالغ</h5>
                                <h2><asp:Label ID="lblTotalAmount" runat="server" Text="0"></asp:Label> ج.م</h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card bg-info text-white">
                            <div class="card-body">
                                <h5 class="card-title">عدد المشتركين</h5>
                                <h2><asp:Label ID="lblSubscriptions" runat="server" Text="0"></asp:Label></h2>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ===== TAB 2: NEW SUBSCRIPTION ===== -->
            <div class="tab-pane-custom" id="tabSubscription">
                <div class="result-box mb-4">
                    <div class="section-header">
                        <i class="fas fa-ticket-alt"></i> إضافة اشتراك جديد
                    </div>

                    <!-- First row - Dropdowns -->
                    <div class="row g-3">
                        <!-- رقم العربية - Dropdown -->
                        <div class="col-md-6">
                            <label class="form-label">رقم العربية <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlSubCar" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                DataTextField="car_number_display"
                                DataValueField="car_number_id">
                                <asp:ListItem Text="-- اختر رقم العربية --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <!-- البوابة Dropdown -->
                        <div class="col-md-6">
                            <label class="form-label">البوابة <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlSubGate" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                DataTextField="gate_display"
                                DataValueField="gate_id">
                                <asp:ListItem Text="-- اختر البوابة --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Second row - Input fields -->
                    <div class="row g-3 mt-3">
                        <!-- مبلغ الاشتراك -->
                        <div class="col-md-3">
                            <label class="form-label">مبلغ الاشتراك <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtSubAmount" runat="server" 
                                CssClass="form-control enhanced-dropdown"
                                placeholder="المبلغ"
                                TextMode="Number" step="0.01" min="0"></asp:TextBox>
                        </div>

                        <!-- تاريخ البداية -->
                        <div class="col-md-3">
                            <label class="form-label">تاريخ البداية <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtSubStartDate" runat="server" 
                                CssClass="form-control enhanced-dropdown" 
                                TextMode="Date"></asp:TextBox>
                        </div>

                        <!-- تاريخ النهاية -->
                        <div class="col-md-3">
                            <label class="form-label">تاريخ النهاية <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtSubEndDate" runat="server" 
                                CssClass="form-control enhanced-dropdown" 
                                TextMode="Date"></asp:TextBox>
                        </div>

                        <!-- ملاحظات -->
                        <div class="col-md-3">
                            <label class="form-label">ملاحظات</label>
                            <asp:TextBox ID="txtSubNotes" runat="server" 
                                CssClass="form-control enhanced-dropdown"
                                placeholder="ملاحظات"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Third row - Buttons -->
                    <div class="row g-3 mt-3">
                        <div class="col-md-12 d-flex align-items-end">
                            <asp:Button ID="btnSaveSubscription" runat="server" Text="حفظ الاشتراك" 
                                CssClass="btn btn-save me-2" style="width: 150px;" OnClick="btnSaveSubscription_Click" />
                            <asp:Button ID="btnClearSubscription" runat="server" Text="مسح" 
                                CssClass="btn btn-clear" style="width: 150px;" OnClick="btnClearSubscription_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- ===== TAB 3: GATES MANAGEMENT ===== -->
            <div class="tab-pane-custom" id="tabGates">
                <div class="result-box mb-4">
                    <div class="section-header">
                        <i class="fas fa-road"></i> إدارة البوابات
                    </div>
                    
                    <div class="row g-3">
                        <div class="col-md-3">
                            <label class="form-label">اسم البوابة <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtGateName" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل اسم البوابة"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">الموقع</label>
                            <asp:TextBox ID="txtGateLocation" runat="server" CssClass="form-control enhanced-dropdown" placeholder="الموقع"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">قيمة العبور <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtGateAmount" runat="server" CssClass="form-control enhanced-dropdown" placeholder="القيمة" TextMode="Number" step="0.01"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">&nbsp;</label>
                            <asp:Button ID="btnSaveGate" runat="server" Text="حفظ البوابة" CssClass="btn btn-save w-100" OnClick="btnSaveGate_Click" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">&nbsp;</label>
                            <asp:Button ID="btnClearGate" runat="server" Text="مسح" CssClass="btn btn-clear w-100" OnClick="btnClearGate_Click" />
                        </div>
                    </div>
                    
                    <!-- قائمة البوابات -->
                    <div class="row g-3 mt-3">
                        <div class="col-md-12">
                            <div class="table-responsive">
                                <asp:GridView ID="gvGates" runat="server" 
                                    CssClass="table table-striped table-bordered table-hover"
                                    AutoGenerateColumns="false"
                                    EmptyDataText="لا توجد بوابات"
                                    AllowPaging="true" PageSize="10"
                                    OnPageIndexChanging="gvGates_PageIndexChanging"
                                    OnRowCommand="gvGates_RowCommand"
                                    GridLines="None">
                                    <Columns>
                                        <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                                        <asp:BoundField DataField="gate_name" HeaderText="اسم البوابة" />
                                        <asp:BoundField DataField="location" HeaderText="الموقع" />
                                        <asp:BoundField DataField="toll_amount" HeaderText="قيمة العبور" DataFormatString="{0:N2} ج.م" />
                                        <asp:TemplateField HeaderText="الحالة">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" 
                                                    CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "badge-subscribed" : "badge-secondary" %>'
                                                    Text='<%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="الإجراءات">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEditGate" runat="server" 
                                                    CommandName="EditGate" 
                                                    CommandArgument='<%# Eval("gate_id") %>'
                                                    CssClass="btn btn-warning btn-sm me-1">
                                                    <i class="fas fa-edit"></i> تعديل
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnToggleStatus" runat="server" 
                                                    CommandName="ToggleStatus" 
                                                    CommandArgument='<%# Eval("gate_id") %>'
                                                    CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn btn-danger btn-sm" : "btn btn-success btn-sm" %>'>
                                                    <i class='<%# Convert.ToBoolean(Eval("is_active")) ? "fas fa-ban" : "fas fa-check" %>'></i>
                                                    <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
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
                </div>
            </div>

        </div><!-- /.tab-content-custom -->

        <!-- Hidden fields -->
        <asp:HiddenField ID="hdnGateId" runat="server" Value="0" />

    </div>

    <script type="text/javascript">
        // ===== Tab Switching =====

        /**
         * Safely returns the hidden field that tracks the active tab,
         * narrowed to HTMLInputElement so `.value` is valid.
         * @returns {HTMLInputElement|null}
         */
        function getActiveTabField() {
            var el = document.getElementById('<%= hfActiveTab.ClientID %>');
            return el instanceof HTMLInputElement ? el : null;
        }

        /**
         * @param {string} tabId
         */
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

            var activeTabField = getActiveTabField();
            if (activeTabField) {
                activeTabField.value = tabId;
            }
        }

        // Store current tab before any postback
        function storeCurrentTab() {
            var activeTabField = getActiveTabField();
            if (activeTabField) {
                var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
                tabs.forEach(function (tab) {
                    if (tab.classList.contains('active')) {
                        var tabId = tab.getAttribute('data-tab');
                        if (tabId && activeTabField) {
                            activeTabField.value = tabId;
                        }
                    }
                });
            }
            return true;
        }

        document.addEventListener('DOMContentLoaded', function () {
            var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
            var activeTabField = getActiveTabField();

            // Click handlers
            tabs.forEach(function (tab) {
                tab.addEventListener('click', function (e) {
                    e.preventDefault();
                    var tabId = tab.getAttribute('data-tab');
                    if (tabId) {
                        activateTab(tabId);
                    }
                });
            });

            // Restore active tab after postback
            if (activeTabField && activeTabField.value) {
                activateTab(activeTabField.value);
            }

            // If search results are visible, switch to search tab
            var searchResultsEl = document.getElementById('<%= searchResults.ClientID %>');
            if (searchResultsEl instanceof HTMLElement && searchResultsEl.style.display !== 'none') {
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
