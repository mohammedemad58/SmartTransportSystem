<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
    CodeBehind="employee_subscriber_management.aspx.cs" Inherits="car_WebApplication1.employee_subscriber_management" %>

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
        .result-box {
            border: 2px solid #000;
            border-radius: 10px;
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
        .btn-reset {
            background: #6c757d;
            color: white;
            border: none;
            height: 45px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        .btn-reset:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        .btn-employee-search {
            background-color: #c8102e;
            color: white;
            border: none;
            height: 45px;
            width: 100%;
            font-size: 14px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        .btn-employee-search:hover {
            background-color: #a50d25;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
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
        }
        .enhanced-dropdown:hover {
            border-color: #c8102e;
        }
        .enhanced-dropdown:focus {
            border-color: #c8102e;
            box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1);
            outline: none;
        }
        .employee-info {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-top: 15px;
            border-right: 4px solid #c8102e;
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
        .stat-card.bg-success {
            background: linear-gradient(135deg, #28a745 0%, #1e7e34 100%) !important;
            color: white;
        }
        .stat-card.bg-info {
            background: linear-gradient(135deg, #17a2b8 0%, #117a8b 100%) !important;
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
        .badge-subscribed {
            background-color: #28a745;
            color: white;
            padding: 5px 10px;
            border-radius: 20px;
            font-size: 12px;
        }
        .badge-pay-per-use {
            background-color: #ffc107;
            color: #2c3e50;
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
        .search-employee-row {
            display: flex;
            gap: 10px;
            align-items: flex-end;
        }
        .search-employee-row .form-group {
            flex: 1;
        }
        .search-employee-row button {
            flex: 0 0 100px;
        }
        .ms-2 {
            margin-right: 8px;
        }
        .mt-3 {
            margin-top: 15px;
        }
        .mb-3 {
            margin-bottom: 15px;
        }
        
        /* Validation Styles */
        .text-danger {
            color: #dc3545 !important;
            font-size: 12px;
            margin-top: 5px;
            display: block;
        }
        
        .validation-error {
            border-color: #dc3545 !important;
            border-width: 2px !important;
        }
        
        @media (max-width: 768px) {
            .search-employee-row {
                flex-direction: column;
            }
            .search-employee-row button {
                flex: 1;
            }
            .stat-card h3 {
                font-size: 24px;
            }
        }
        .btn-info {
            background-color: #17a2b8 !important;
            border-color: #17a2b8 !important;
            color: white !important;
        }

        .btn-info:hover {
            background-color: #138496 !important;
            border-color: #138496 !important;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }

        .text-muted {
            font-size: 12px;
            margin-top: 5px;
            display: block;
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

    <div class="container mt-4" dir="rtl">
        <!-- عنوان الصفحة -->
        <div class="text-center">
            <h2 class="page-title">
                <i class="fas fa-users" style="color: #c8102e; margin-left: 10px;"></i>
                إدارة الموظفين والمشتركين
            </h2>
        </div>

        <!-- رسائل التنبيه -->
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <!-- Hidden field to remember active tab across postbacks -->
        <asp:HiddenField ID="hfActiveTab" runat="server" Value="tabAddEmployee" />

        <!-- ===== TABS ===== -->
        <div class="nav-tabs-custom" id="mainTabs">
            <div class="nav-item">
                <button class="nav-link active" data-tab="tabAddEmployee">
                    <i class="fas fa-user-plus"></i> موظف جديد
                </button>
            </div>
            <div class="nav-item">
                <button class="nav-link" data-tab="tabAddSubscription">
                    <i class="fas fa-ticket-alt"></i> اشتراك جديد
                </button>
            </div>
            <div class="nav-item">
                <button class="nav-link" data-tab="tabSearchResults">
                    <i class="fas fa-search"></i> البحث والنتائج
                </button>
            </div>
        </div>

        <div class="tab-content-custom">

            <!-- ===== TAB 1: ADD EMPLOYEE ===== -->
            <div class="tab-pane-custom active" id="tabAddEmployee">
                <div class="result-box mb-4">
                    <div class="section-header">
                        <i class="fas fa-user-plus"></i> إضافة موظف جديد
                    </div>

                    <div class="row g-3">
                        <div class="col-md-4">
                            <label class="form-label">اسم الموظف <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtEmpName" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل اسم الموظف"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">الوظيفة <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlEmpPosition" runat="server" CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="-- اختر الوظيفة --" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">البريد الإلكتروني</label>
                            <asp:TextBox ID="txtEmpEmail" runat="server" CssClass="form-control enhanced-dropdown" placeholder="example@domain.com" TextMode="Email"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row g-3 mt-2">
                        <div class="col-md-4">
                            <label class="form-label">رقم الهاتف</label>
                            <asp:TextBox ID="txtEmpPhone" runat="server" CssClass="form-control enhanced-dropdown" placeholder="رقم الهاتف"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">الرقم القومي <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtEmpNationalId" runat="server" CssClass="form-control enhanced-dropdown" placeholder="الرقم القومي (14 رقم يبدأ بـ 2 أو 3)"></asp:TextBox>
                        </div>
                    </div>

                    <!-- أزرار مرتبة بشكل أفضل -->
                    <div class="row g-3 mt-3">
                        <div class="col-md-3">
                            <asp:Button ID="btnSaveEmployee" runat="server" Text="حفظ الموظف" CssClass="btn btn-save w-100" OnClick="btnSaveEmployee_Click" />
                        </div>
                        <div class="col-md-3">
                            <asp:Button ID="btnClearEmployee" runat="server" Text="مسح الحقول" CssClass="btn btn-clear w-100" OnClick="btnClearEmployee_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- ===== TAB 2: ADD SUBSCRIPTION ===== -->
            <div class="tab-pane-custom" id="tabAddSubscription">
                <div class="result-box mb-4">
                    <div class="section-header">
                        <i class="fas fa-ticket-alt"></i> إضافة اشتراك جديد
                    </div>

                    <div class="row g-3">
                        <!-- شريط بحث الموظفين -->
                        <div class="col-md-12">
                            <label class="form-label">البحث عن موظف</label>
                            <div class="search-employee-row">
                                <div class="form-group" style="flex: 1;">
                                    <asp:TextBox ID="txtEmployeeSearch" runat="server" 
                                        CssClass="form-control enhanced-dropdown" 
                                        placeholder="أدخل اسم الموظف للبحث..."></asp:TextBox>
                                </div>
                                <button type="button" class="btn btn-employee-search" onclick="searchEmployee()">
                                    <i class="fas fa-search"></i> بحث
                                </button>
                                <button type="button" class="btn btn-secondary" onclick="clearEmployeeSearch()" style="background-color: #6c757d; color: white; border: none; height: 45px; border-radius: 8px;">
                                    <i class="fas fa-times"></i> مسح
                                </button>
                            </div>
                            <small class="text-muted">(اختياري - يمكنك البحث أو اختيار موظف من القائمة)</small>
                        </div>
                    </div>

                    <!-- نتائج البحث عن الموظفين -->
                    <div id="employeeSearchResults" runat="server" visible="false" class="mt-3 mb-3">
                        <div class="table-responsive">
                            <asp:GridView ID="gvEmployeeSearchResults" runat="server" 
                                CssClass="table table-striped table-bordered table-hover"
                                AutoGenerateColumns="false"
                                EmptyDataText="لا توجد نتائج للبحث"
                                OnRowCommand="gvEmployeeSearchResults_RowCommand"
                                GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="employee_name" HeaderText="اسم الموظف" />
                                    <asp:BoundField DataField="employee_position" HeaderText="الوظيفة" />
                                    <asp:BoundField DataField="phone_number" HeaderText="الهاتف" />
                                    <asp:TemplateField HeaderText="اختيار">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnSelectEmployee" runat="server" 
                                                CommandName="SelectEmployee" 
                                                CommandArgument='<%# Eval("employee_id") %>'
                                                CssClass="btn btn-sm btn-success">
                                                <i class="fas fa-check"></i> اختيار
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="table-header" BackColor="#c8102e" ForeColor="White" />
                                <RowStyle CssClass="table-row" />
                                <AlternatingRowStyle CssClass="table-alt-row" />
                            </asp:GridView>
                        </div>
                    </div>

                    <!-- بديل: قائمة منسدلة للموظفين (إذا لم يستخدم البحث) -->
                    <div class="row g-3 mt-2">
                        <div class="col-md-12">
                            <label class="form-label">اختر موظف من القائمة <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlEmployee" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged">
                                <asp:ListItem Text="-- اختر موظف --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- معلومات الموظف المختار -->
                    <div id="employeeInfo" runat="server" class="employee-info" visible="false">
                        <div class="row">
                            <div class="col-md-4">
                                <strong>الاسم:</strong> <asp:Label ID="lblSelectedEmployeeName" runat="server" Text=""></asp:Label>
                            </div>
                            <div class="col-md-4">
                                <strong>الوظيفة:</strong> <asp:Label ID="lblSelectedEmployeePosition" runat="server" Text=""></asp:Label>
                            </div>
                            <div class="col-md-4">
                                <strong>الهاتف:</strong> <asp:Label ID="lblSelectedEmployeePhone" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="row g-3 mt-3">
                        <div class="col-md-4">
                            <label class="form-label">خط السير <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlRoute" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                DataTextField="route_name"
                                DataValueField="route_id">
                                <asp:ListItem Text="-- اختر خط السير --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">تاريخ البداية <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtSubStartDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">تاريخ النهاية <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtSubEndDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">اداره</label>
                            <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control enhanced-dropdown" placeholder="اداره"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">الوظيفة</label>
                            <asp:TextBox ID="txtSubPosition" runat="server" CssClass="form-control enhanced-dropdown" placeholder="الوظيفة"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row g-3 mt-2">
                        <div class="col-md-4">
                            <label class="form-label">العنوان</label>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control enhanced-dropdown" placeholder="العنوان"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">ملاحظات</label>
                            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control enhanced-dropdown" placeholder="ملاحظات إضافية"></asp:TextBox>
                        </div>
                        <div class="col-md-4 d-flex align-items-end">
                            <asp:Button ID="btnSaveSubscription" runat="server" Text="حفظ الاشتراك" CssClass="btn btn-save w-100 me-2" OnClick="btnSaveSubscription_Click" />
                            <asp:Button ID="btnClearSubscription" runat="server" Text="مسح" CssClass="btn btn-clear w-100" OnClick="btnClearSubscription_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- ===== TAB 3: SEARCH & RESULTS ===== -->
            <div class="tab-pane-custom" id="tabSearchResults">

                <!-- قسم البحث العام -->
                <div class="search-box">
                    <h5 class="fw-bold mb-3">
                        <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                        بحث عام
                    </h5>
                    <div class="row g-3">
                        <div class="col-md-3">
                            <label class="form-label">اسم الموظف</label>
                            <asp:TextBox ID="txtSearchEmployee" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل اسم الموظف"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">نوع البحث</label>
                            <asp:DropDownList ID="ddlSearchType" runat="server" CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="الكل (موظفين ومشتركين)" Value="all"></asp:ListItem>
                                <asp:ListItem Text="موظفين فقط" Value="employees"></asp:ListItem>
                                <asp:ListItem Text="مشتركين فقط" Value="subscribers"></asp:ListItem>
                                <asp:ListItem Text="سائقين مشتركين فقط" Value="drivers_subscribers"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">الوظيفة</label>
                            <asp:DropDownList ID="ddlSearchPosition" runat="server" CssClass="form-control enhanced-dropdown">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">من تاريخ</label>
                            <asp:TextBox ID="txtSubFromDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">إلى تاريخ</label>
                            <asp:TextBox ID="txtSubToDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-3 d-flex align-items-end">
                            <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-search w-100 me-2" OnClick="btnSearch_Click" />
                            <asp:Button ID="btnReset" runat="server" Text="إعادة تعيين" CssClass="btn btn-reset w-100" OnClick="btnReset_Click" />
                        </div>
                    </div>
                </div>

                <!-- نتائج الموظفين -->
                <div id="employeeResults" runat="server" class="result-box mb-4" visible="false">
                    <h5 class="fw-bold mb-3">
                        <i class="fas fa-users" style="color: #c8102e; margin-left: 8px;"></i>
                        نتائج الموظفين
                    </h5>
                    <div class="table-responsive">
                        <asp:GridView ID="gvEmployees" runat="server" 
                            CssClass="table table-striped table-bordered table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد نتائج للموظفين"
                            AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvEmployees_PageIndexChanging"
                            OnRowCommand="gvEmployees_RowCommand"
                            GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                                <asp:BoundField DataField="employee_name" HeaderText="اسم الموظف" />
                                <asp:BoundField DataField="employee_position" HeaderText="الوظيفة" />
                                <asp:BoundField DataField="phone_number" HeaderText="رقم الهاتف" />
                                <asp:BoundField DataField="email" HeaderText="البريد الإلكتروني" />
                                <asp:BoundField DataField="national_id" HeaderText="الرقم القومي" />
                                <asp:TemplateField HeaderText="تعديل">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEditEmployee" runat="server" 
                                            CommandName="EditEmployee" 
                                            CommandArgument='<%# Eval("employee_id") %>'
                                            CssClass="btn btn-warning btn-sm">
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

                <!-- نتائج المشتركين -->
                <div id="subscriberResults" runat="server" class="result-box" visible="false">
                    <h5 class="fw-bold mb-3">
                        <i class="fas fa-ticket-alt" style="color: #c8102e; margin-left: 8px;"></i>
                        نتائج المشتركين
                    </h5>
                    <div class="table-responsive">
                        <asp:GridView ID="gvSubscribers" runat="server" 
                            CssClass="table table-striped table-bordered table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد نتائج للمشتركين"
                            AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvSubscribers_PageIndexChanging"
                            GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                                <asp:BoundField DataField="name" HeaderText="اسم المشترك" />
                                <asp:BoundField DataField="position" HeaderText="الوظيفة" />
                                <asp:BoundField DataField="phone" HeaderText="الهاتف" />
                                <asp:BoundField DataField="route_name" HeaderText="خط السير" />
                                <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="department" HeaderText="اداره" />
                                <asp:BoundField DataField="address" HeaderText="العنوان" />
                            </Columns>
                            <HeaderStyle CssClass="table-header" BackColor="#c8102e" ForeColor="White" />
                            <RowStyle CssClass="table-row" />
                            <AlternatingRowStyle CssClass="table-alt-row" />
                            <PagerStyle CssClass="grid-pager" />
                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div><!-- /.tab-content-custom -->

        <!-- إحصائيات سريعة (تظهر فقط بعد البحث) -->
        <div id="statsCardsRow" class="row mt-4 mb-4" style="display: none;">
            <div class="col-md-4">
                <div class="stat-card bg-danger">
                    <i class="fas fa-users fa-2x"></i>
                    <h3><asp:Label ID="lblTotalEmployees" runat="server" Text="0"></asp:Label></h3>
                    <p>إجمالي الموظفين</p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="stat-card bg-success">
                    <i class="fas fa-ticket-alt fa-2x"></i>
                    <h3><asp:Label ID="lblTotalSubscribers" runat="server" Text="0"></asp:Label></h3>
                    <p>إجمالي المشتركين</p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="stat-card bg-info">
                    <i class="fas fa-check-circle fa-2x"></i>
                    <h3><asp:Label ID="lblActiveSubscriptions" runat="server" Text="0"></asp:Label></h3>
                    <p>اشتراكات نشطة</p>
                </div>
            </div>
        </div>

        <!-- Hidden Field for Employee ID (for editing) -->
        <asp:HiddenField ID="hdnEmployeeId" runat="server" Value="0" />

    </div>

    <!-- Client-Side Validation JavaScript -->
    <script type="text/javascript">
        // Helper Functions
        function isNumber(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }

        // Validate Egyptian National ID (14 digits, starts with 2 or 3)
        function validateNationalId(nationalId) {
            if (nationalId === "") return false; // Required field
            var nationalIdRegex = /^[23]\d{13}$/;
            return nationalIdRegex.test(nationalId);
        }

        // Validate Egyptian Phone Number (11 digits, starts with 010/011/012/015)
        function validatePhone(phone) {
            if (phone === "") return true; // Optional field
            var phoneRegex = /^(010|011|012|015)\d{8}$/;
            return phoneRegex.test(phone);
        }

        // Validate Email format
        function validateEmail(email) {
            if (email === "") return true; // Optional field
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(email);
        }

        // Show error message
        function showError(fieldId, message) {
            var field = document.getElementById(fieldId);
            if (!field) return;

            var errorSpan = document.getElementById(fieldId + "_error");
            if (!errorSpan) {
                errorSpan = document.createElement("span");
                errorSpan.id = fieldId + "_error";
                errorSpan.className = "text-danger";
                field.parentNode.appendChild(errorSpan);
            }
            errorSpan.innerHTML = message;
            field.classList.add("validation-error");
        }

        function clearError(fieldId) {
            var field = document.getElementById(fieldId);
            if (!field) return;

            var errorSpan = document.getElementById(fieldId + "_error");
            if (errorSpan) {
                errorSpan.innerHTML = "";
            }
            field.classList.remove("validation-error");
        }

        // Validate Employee Form (Client-side)
        function validateEmployeeForm() {
            var isValid = true;

            // 1. Employee Name (Required)
            var empName = document.getElementById('<%= txtEmpName.ClientID %>');
            if (empName.value.trim() === "") {
                showError('<%= txtEmpName.ClientID %>', 'اسم الموظف مطلوب');
                isValid = false;
            } else {
                clearError('<%= txtEmpName.ClientID %>');
            }

            // 2. Position (Required)
            var position = document.getElementById('<%= ddlEmpPosition.ClientID %>');
            if (position.value === "") {
                showError('<%= ddlEmpPosition.ClientID %>', 'الرجاء اختيار الوظيفة');
                isValid = false;
            } else {
                clearError('<%= ddlEmpPosition.ClientID %>');
            }

            // 3. National ID (Required - Unique Identifier)
            var nationalId = document.getElementById('<%= txtEmpNationalId.ClientID %>');
            if (!validateNationalId(nationalId.value.trim())) {
                showError('<%= txtEmpNationalId.ClientID %>', 'الرقم القومي يجب أن يكون 14 رقم ويبدأ بـ 2 أو 3');
                isValid = false;
            } else {
                clearError('<%= txtEmpNationalId.ClientID %>');
            }

            // 4. Email (Optional but must be valid if provided)
            var email = document.getElementById('<%= txtEmpEmail.ClientID %>');
            if (email.value.trim() !== "" && !validateEmail(email.value.trim())) {
                showError('<%= txtEmpEmail.ClientID %>', 'البريد الإلكتروني غير صحيح (example@domain.com)');
                isValid = false;
            } else {
                clearError('<%= txtEmpEmail.ClientID %>');
            }

            // 5. Phone (Optional but must be valid if provided)
            var phone = document.getElementById('<%= txtEmpPhone.ClientID %>');
            if (phone.value.trim() !== "" && !validatePhone(phone.value.trim())) {
                showError('<%= txtEmpPhone.ClientID %>', 'رقم الهاتف يجب أن يكون 11 رقم ويبدأ بـ 010, 011, 012, أو 015');
                isValid = false;
            } else {
                clearError('<%= txtEmpPhone.ClientID %>');
            }

            return isValid;
        }

        // Validate Subscription Form (Client-side)
        function validateSubscriptionForm() {
            var isValid = true;

            // 1. Employee Selection - Check dropdown only
            var ddlEmployee = document.getElementById('<%= ddlEmployee.ClientID %>');
            if (ddlEmployee.value === "0") {
                alert("الرجاء اختيار موظف من القائمة");
                showError('<%= ddlEmployee.ClientID %>', 'الرجاء اختيار موظف');
                isValid = false;
            } else {
                clearError('<%= ddlEmployee.ClientID %>');
            }

            // 2. Route (Required)
            var route = document.getElementById('<%= ddlRoute.ClientID %>');
            if (route.value === "0") {
                showError('<%= ddlRoute.ClientID %>', 'الرجاء اختيار خط السير');
                isValid = false;
            } else {
                clearError('<%= ddlRoute.ClientID %>');
            }
            
            // 3. Start Date (Required)
            var startDate = document.getElementById('<%= txtSubStartDate.ClientID %>');
            if (startDate.value === "") {
                showError('<%= txtSubStartDate.ClientID %>', 'تاريخ البداية مطلوب');
                isValid = false;
            } else {
                clearError('<%= txtSubStartDate.ClientID %>');
            }
            
            // 4. End Date (Required)
            var endDate = document.getElementById('<%= txtSubEndDate.ClientID %>');
            if (endDate.value === "") {
                showError('<%= txtSubEndDate.ClientID %>', 'تاريخ النهاية مطلوب');
                isValid = false;
            } else {
                clearError('<%= txtSubEndDate.ClientID %>');
            }
            
            // 5. Date Logic (End Date must be after Start Date)
            if (startDate.value !== "" && endDate.value !== "") {
                var start = new Date(startDate.value);
                var end = new Date(endDate.value);
                if (end <= start) {
                    showError('<%= txtSubEndDate.ClientID %>', 'تاريخ النهاية يجب أن يكون بعد تاريخ البداية');
                    isValid = false;
                }
            }

            return isValid;
        }

        // Restrict input to numbers only
        function restrictToNumbers(inputElement, maxLength) {
            if (!inputElement) return;
            inputElement.addEventListener('input', function() {
                this.value = this.value.replace(/[^\d]/g, '');
                if (maxLength && this.value.length > maxLength) {
                    this.value = this.value.substring(0, maxLength);
                }
            });
        }

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

            // If the general search returned results (employeeResults / subscriberResults
            // were rendered by the server after btnSearch_Click), auto-jump to that tab
            // and reveal the stats cards.
            var employeeResultsEl = document.getElementById('<%= employeeResults.ClientID %>');
            var subscriberResultsEl = document.getElementById('<%= subscriberResults.ClientID %>');
            var statsCardsRow = document.getElementById('statsCardsRow');

            if (employeeResultsEl || subscriberResultsEl) {
                activateTab('tabSearchResults');
                if (statsCardsRow) statsCardsRow.style.display = '';
            } else if (activeTabField && activeTabField.value) {
                activateTab(activeTabField.value);
            }
        });

        // Attach validation to buttons and initialize
        document.addEventListener('DOMContentLoaded', function() {
            // Employee Save Button
            var btnSaveEmployee = document.getElementById('<%= btnSaveEmployee.ClientID %>');
            if (btnSaveEmployee) {
                btnSaveEmployee.addEventListener('click', function(e) {
                    if (!validateEmployeeForm()) {
                        e.preventDefault();
                        return false;
                    }
                });
            }
            
            // Subscription Save Button
            var btnSaveSubscription = document.getElementById('<%= btnSaveSubscription.ClientID %>');
            if (btnSaveSubscription) {
                btnSaveSubscription.addEventListener('click', function(e) {
                    if (!validateSubscriptionForm()) {
                        e.preventDefault();
                        return false;
                    }
                });
            }
            
            // Real-time validation on blur
            var nationalId = document.getElementById('<%= txtEmpNationalId.ClientID %>');
            if (nationalId) {
                nationalId.addEventListener('blur', function() {
                    if (!validateNationalId(this.value)) {
                        showError(this.id, 'الرقم القومي يجب أن يكون 14 رقم ويبدأ بـ 2 أو 3');
                    } else {
                        clearError(this.id);
                    }
                });
                restrictToNumbers(nationalId, 14);
            }
            
            var phone = document.getElementById('<%= txtEmpPhone.ClientID %>');
            if (phone) {
                phone.addEventListener('blur', function() {
                    if (this.value !== "" && !validatePhone(this.value)) {
                        showError(this.id, 'رقم الهاتف غير صحيح (11 رقم يبدأ بـ 010/011/012/015)');
                    } else {
                        clearError(this.id);
                    }
                });
                restrictToNumbers(phone, 11);
            }
            
            var email = document.getElementById('<%= txtEmpEmail.ClientID %>');
            if (email) {
                email.addEventListener('blur', function() {
                    if (this.value !== "" && !validateEmail(this.value)) {
                        showError(this.id, 'البريد الإلكتروني غير صحيح');
                    } else {
                        clearError(this.id);
                    }
                });
            }
            
            var empName = document.getElementById('<%= txtEmpName.ClientID %>');
            if (empName) {
                empName.addEventListener('blur', function() {
                    if (this.value.trim() === "") {
                        showError(this.id, 'اسم الموظف مطلوب');
                    } else {
                        clearError(this.id);
                    }
                });
            }
        });
        
        // Employee search function
        function searchEmployee() {
            var searchText = document.getElementById('<%= txtEmployeeSearch.ClientID %>').value;
            if (searchText.trim() === "") {
                alert("الرجاء إدخال اسم الموظف للبحث");
                return false;
            }

            // Remember we're on the subscription tab before the postback
            var activeTabField = document.getElementById('<%= hfActiveTab.ClientID %>');
            if (activeTabField) activeTabField.value = 'tabAddSubscription';

            // Call server-side method via __doPostBack
            __doPostBack('SearchEmployee', searchText);
            return false;
        }
        
        // Clear employee search
        function clearEmployeeSearch() {
            var searchBox = document.getElementById('<%= txtEmployeeSearch.ClientID %>');
            if (searchBox) {
                searchBox.value = '';
            }

            // Hide search results
            var searchResults = document.getElementById('<%= employeeSearchResults.ClientID %>');
            if (searchResults) {
                searchResults.style.display = 'none';
            }
        }
    </script>

</asp:Content>