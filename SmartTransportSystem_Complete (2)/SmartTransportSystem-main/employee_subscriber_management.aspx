<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
    CodeBehind="employee_subscriber_management.aspx.cs" Inherits="car_WebApplication1.employee_subscriber_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title { color: #000; font-weight: bold; font-size: 28px; margin-bottom: 25px; }
        .search-box { border: 2px solid #000; border-radius: 10px; padding: 20px; background-color: #fff; margin-bottom: 25px; }
        .result-box { border: 2px solid #000; border-radius: 10px; padding: 25px; margin-top: 20px; background-color: #f9f9f9; }
        .form-label { font-weight: bold; margin-bottom: 8px; color: #333; font-size: 14px; }
        .btn-search { background-color: #c8102e; color: #fff; font-weight: bold; height: 45px; font-size: 16px; border: none; border-radius: 8px; }
        .btn-search:hover { background-color: #a50d25; color: #fff; }
        .btn-save { background: #a50d25; color: white; border: none; height: 45px; font-size: 16px; font-weight: bold; border-radius: 8px; transition: all 0.3s ease; }
        .btn-save:hover { transform: translateY(-2px); box-shadow: 0 4px 8px rgba(0,0,0,0.2); }
        .btn-clear { background: #6c757d; color: white; border: none; height: 45px; font-size: 16px; font-weight: bold; border-radius: 8px; transition: all 0.3s ease; }
        .btn-clear:hover { transform: translateY(-2px); box-shadow: 0 4px 8px rgba(0,0,0,0.2); }
        .btn-reset { background: #6c757d; color: white; border: none; height: 45px; font-size: 16px; font-weight: bold; border-radius: 8px; transition: all 0.3s ease; }
        .btn-reset:hover { transform: translateY(-2px); box-shadow: 0 4px 8px rgba(0,0,0,0.2); }
        .section-header { background: linear-gradient(90deg, #c8102e, #ffffff); padding: 10px 15px; border-radius: 8px; margin-bottom: 20px; font-weight: bold; color: #000; font-size: 18px; }
        .section-header i { margin-left: 10px; color: #c8102e; }
        .table-header { background-color: #c8102e; color: white; }
        .table-row:hover { background-color: #f5f5f5; }
        .table-alt-row { background-color: #f9f9f9; }
        .enhanced-dropdown { background: #fff; border: 2px solid #dee2e6; border-radius: 8px; transition: all 0.3s ease; height: 45px; width: 100%; padding: 0 10px; }
        .enhanced-dropdown:hover { border-color: #c8102e; }
        .enhanced-dropdown:focus { border-color: #c8102e; box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1); outline: none; }
        .employee-info { background: #f8f9fa; padding: 15px; border-radius: 8px; margin-top: 15px; border-right: 4px solid #c8102e; }
        .stat-card { background: white; border-radius: 10px; padding: 20px; text-align: center; box-shadow: 0 4px 8px rgba(0,0,0,0.1); transition: all 0.3s ease; }
        .stat-card:hover { transform: translateY(-2px); box-shadow: 0 6px 12px rgba(0,0,0,0.15); }
        .stat-card.bg-danger { background: linear-gradient(135deg, #c8102e 0%, #a50d25 100%) !important; color: white; }
        .stat-card.bg-success { background: linear-gradient(135deg, #28a745 0%, #1e7e34 100%) !important; color: white; }
        .stat-card.bg-info { background: linear-gradient(135deg, #17a2b8 0%, #117a8b 100%) !important; color: white; }
        .stat-card h3 { font-size: 32px; font-weight: 700; margin: 10px 0; }
        .stat-card p { margin: 0; font-size: 14px; opacity: 0.9; }
        .badge-subscribed { background-color: #28a745; color: white; padding: 5px 10px; border-radius: 20px; font-size: 12px; }
        .badge-expired { background-color: #dc3545; color: white; padding: 5px 10px; border-radius: 20px; font-size: 12px; }
        .badge-secondary { background-color: #6c757d; color: white; padding: 5px 10px; border-radius: 20px; font-size: 12px; }
        .grid-pager { background: #f8f9fa; padding: 15px; text-align: center; border-radius: 0 0 10px 10px; }
        .grid-pager table { margin: 0 auto; }
        .grid-pager td { padding: 5px 10px; border: none; }
        .grid-pager a { color: #c8102e; text-decoration: none; padding: 5px 10px; border-radius: 5px; }
        .grid-pager a:hover { background: #c8102e; color: white; }
        .grid-pager span { background: #c8102e; color: white; padding: 5px 10px; border-radius: 5px; }
        .text-danger { color: #dc3545 !important; font-size: 12px; margin-top: 5px; display: block; }
        .validation-error { border-color: #dc3545 !important; border-width: 2px !important; }
        .btn-warning { background-color: #ffc107; border-color: #ffc107; color: #333; }
        .btn-warning:hover { background-color: #e0a800; border-color: #e0a800; color: #333; }
        .text-muted { font-size: 12px; margin-top: 5px; display: block; }

        .search-employee-row { display: flex; gap: 10px; align-items: flex-end; }
        .search-employee-row .form-group { flex: 1; }
        .search-employee-row button { flex: 0 0 100px; }

        .search-panel {
            background: #f8f9fa;
            border: 2px solid #dee2e6;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
        }
        .search-panel-title {
            font-weight: bold;
            color: #c8102e;
            font-size: 16px;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid #dee2e6;
        }

        .nav-tabs-custom { border-bottom: 2px solid #dee2e6; margin-bottom: 20px; display: flex; flex-wrap: wrap; justify-content: center; gap: 2px; }
        .nav-tabs-custom .nav-item { margin-bottom: -2px; }
        .nav-tabs-custom .nav-link { background: transparent; border: none; color: #495057; font-weight: bold; padding: 12px 24px; border-radius: 0; cursor: pointer; transition: all 0.3s; position: relative; font-size: 16px; }
        .nav-tabs-custom .nav-link:hover { color: #c8102e; background: #f8f9fa; }
        .nav-tabs-custom .nav-link.active { color: #c8102e; border-bottom: 3px solid #c8102e; background: transparent; }
        .tab-content-custom { padding: 5px 0; }
        .tab-pane-custom { display: none; animation: fadeInTab 0.4s; }
        .tab-pane-custom.active { display: block; }
        @keyframes fadeInTab { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 768px) {
            .search-employee-row { flex-direction: column; }
            .search-employee-row button { flex: 1; }
            .stat-card h3 { font-size: 24px; }
            .nav-tabs-custom .nav-link { font-size: 14px !important; padding: 10px 15px !important; }
        }
    </style>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css"/>

    <div class="container mt-4" dir="rtl">
        <div class="text-center">
            <h2 class="page-title">
                <i class="fas fa-users" style="color: #c8102e; margin-left: 10px;"></i>
  تسكين المشتركين - الخط  </h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <asp:HiddenField ID="hfActiveTab" runat="server" Value="tabEmployees" />

        <!-- ===== TABS: Employees / Subscribers ===== -->
        <div class="nav-tabs-custom" id="mainTabs">
            <div class="nav-item"><button type="button" class="nav-link active" data-tab="tabEmployees"><i class="fas fa-users"></i> الموظفين</button></div>
            <div class="nav-item"><button type="button" class="nav-link" data-tab="tabSubscribers"><i class="fas fa-ticket-alt"></i> المشتركين</button></div>
        </div>

        <div class="tab-content-custom">

            <!-- ============================================================ -->
            <!-- ==================== TAB 1: EMPLOYEES ====================== -->
            <!-- ============================================================ -->
            <div class="tab-pane-custom active" id="tabEmployees">

                <%-- ===== Search panel ===== --%>
                <div class="search-panel">
                    <div class="search-panel-title">
                        <i class="fas fa-search"></i> البحث عن موظفين
                    </div>
                    <div class="row g-3">
                        <div class="col-md-4">
                            <label class="form-label">اسم الموظف</label>
                            <asp:TextBox ID="txtSearchEmployee" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل اسم الموظف"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">الوظيفة</label>
                            <asp:DropDownList ID="ddlSearchPosition" runat="server" CssClass="form-control enhanced-dropdown"></asp:DropDownList>
                        </div>
                        <div class="col-md-4 d-flex align-items-end">
                            <asp:Button ID="btnSearchEmployees" runat="server" Text="بحث" CssClass="btn btn-search w-100 me-2" OnClick="btnSearchEmployees_Click" />
                            <asp:Button ID="btnResetEmployees" runat="server" Text="إعادة تعيين" CssClass="btn btn-reset w-100" OnClick="btnResetEmployees_Click" />
                        </div>
                    </div>
                </div>

                <%-- ===== Employees results grid ===== --%>
                <div id="employeeResults" runat="server" class="result-box mb-4" visible="false">
                    <h5 class="fw-bold mb-3"><i class="fas fa-users" style="color: #c8102e; margin-left: 8px;"></i> نتائج الموظفين</h5>
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
                                <asp:BoundField DataField="address" HeaderText="العنوان" />
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

                <%-- ===== Add/Edit employee form ===== --%>
                <div class="result-box mb-4">
                    <div class="section-header">
                        <i class="fas fa-user-plus"></i>
                        <asp:Label ID="lblEmployeeFormTitle" runat="server" Text="إضافة موظف جديد"></asp:Label>
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
                        <div class="col-md-4">
                            <label class="form-label">العنوان</label>
                            <asp:TextBox ID="txtEmpAddress" runat="server" CssClass="form-control enhanced-dropdown" placeholder="العنوان"></asp:TextBox>
                        </div>
                    </div>

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

            <!-- ============================================================ -->
            <!-- =================== TAB 2: SUBSCRIBERS ===================== -->
            <!-- ============================================================ -->
            <div class="tab-pane-custom" id="tabSubscribers">

                <%-- ===== Search panel ===== --%>
                <div class="search-panel">
                    <div class="search-panel-title">
                        <i class="fas fa-search"></i> البحث عن مشتركين
                    </div>
                    <div class="row g-3">
                        <div class="col-md-3">
                            <label class="form-label">اسم المشترك</label>
                            <asp:TextBox ID="txtSearchSubscriber" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل اسم المشترك"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">خط السير</label>
                            <asp:DropDownList ID="ddlSearchRoute" runat="server" CssClass="form-control enhanced-dropdown"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">حالة الاشتراك</label>
                            <asp:DropDownList ID="ddlSearchStatus" runat="server" CssClass="form-control enhanced-dropdown">
                                <asp:ListItem Text="الكل" Value=""></asp:ListItem>
                                <asp:ListItem Text="نشط" Value="active"></asp:ListItem>
                                <asp:ListItem Text="منتهي" Value="expired"></asp:ListItem>
                                <asp:ListItem Text="مخفي" Value="hidden"></asp:ListItem>
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
                        <div class="col-md-12 d-flex align-items-end mt-3">
                            <asp:Button ID="btnSearchSubscribers" runat="server" Text="بحث" CssClass="btn btn-search w-100 me-2" OnClick="btnSearchSubscribers_Click" />
                            <asp:Button ID="btnResetSubscribers" runat="server" Text="إعادة تعيين" CssClass="btn btn-reset w-100" OnClick="btnResetSubscribers_Click" />
                        </div>
                    </div>
                </div>

                <%-- ===== Subscribers results grid ===== --%>
                <div id="subscriberResults" runat="server" class="result-box mb-4" visible="false">
                    <h5 class="fw-bold mb-3"><i class="fas fa-ticket-alt" style="color: #c8102e; margin-left: 8px;"></i> نتائج المشتركين</h5>
                    <div class="table-responsive">
                        <asp:GridView ID="gvSubscribers" runat="server" 
                            CssClass="table table-striped table-bordered table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد نتائج للمشتركين"
                            AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvSubscribers_PageIndexChanging"
                            OnRowCommand="gvSubscribers_RowCommand"
                            GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                                <asp:BoundField DataField="name" HeaderText="اسم المشترك" />
                                <asp:BoundField DataField="position" HeaderText="الوظيفة" />
                                <asp:BoundField DataField="phone" HeaderText="الهاتف" />
                                <asp:BoundField DataField="route_name" HeaderText="خط السير" />
                                <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="address" HeaderText="العنوان" />
                                <asp:TemplateField HeaderText="الحالة">
                                    <ItemTemplate>
                                        <span class='<%# GetSubStatusClass(Eval("sub_status")) %>'><%# Eval("sub_status") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="تعديل">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEditSubscription" runat="server" 
                                            CommandName="EditSubscription" 
                                            CommandArgument='<%# Eval("subscriber_id") %>'
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

                <%-- ===== Add/Edit subscription form (MULTI-EMPLOYEE) ===== --%>
                <div class="result-box mb-4">
                    <div class="section-header">
                        <i class="fas fa-ticket-alt"></i>
                        <asp:Label ID="lblSubscriptionFormTitle" runat="server" Text="إضافة اشتراك جديد"></asp:Label>
                    </div>

                    <!-- خط السير + filter -->
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label">خط السير <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlRoute" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                DataTextField="route_name"
                                DataValueField="route_id">
                                <asp:ListItem Text="-- اختر خط السير --" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">فلترة حسب الوظيفة (اختياري)</label>
                            <asp:DropDownList ID="ddlEmpPositionFilter" runat="server"
                                CssClass="form-control enhanced-dropdown"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlEmpPositionFilter_SelectedIndexChanged">
                                <asp:ListItem Text="كل الوظائف" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- MULTI-EMPLOYEE PICKER -->
                    <div class="row g-3 mt-2">
                        <div class="col-md-12">
                            <label class="form-label">اختر الموظفين <span class="text-danger">*</span></label>
                            <div class="input-group mb-2">
                                <asp:TextBox ID="txtEmpPickerSearch" runat="server" CssClass="form-control enhanced-dropdown" placeholder="بحث في الموظفين..."></asp:TextBox>
                                <asp:Button ID="btnEmpPickerSearch" runat="server" Text="بحث" CssClass="btn btn-search" OnClick="btnEmpPickerSearch_Click" />
                            </div>

                            <div class="border rounded p-2" style="max-height:260px; overflow:auto; background:#fff;">
                                <asp:CheckBoxList ID="cblEmployees" runat="server"
                                    CssClass="w-100"
                                    RepeatLayout="UnorderedList"
                                    RepeatDirection="Vertical"
                                    CellPadding="4"
                                    CellSpacing="4">
                                </asp:CheckBoxList>
                            </div>

                            <div class="mt-2 d-flex flex-wrap gap-2 align-items-center">
                                <asp:Button ID="btnSelectAllEmployees" runat="server" Text="تحديد الكل" CssClass="btn btn-secondary btn-sm" OnClick="btnSelectAllEmployees_Click" />
                                <asp:Button ID="btnClearSelectedEmployees" runat="server" Text="إلغاء التحديد" CssClass="btn btn-secondary btn-sm" OnClick="btnClearSelectedEmployees_Click" />
                                <asp:Label ID="lblSelectedCount" runat="server" CssClass="text-muted" Text="عدد المحددين: 0"></asp:Label>
                            </div>

                            <small class="text-muted d-block mt-2">
                                <i class="fas fa-info-circle"></i>
                                عند الوصول لعدد (4 أو 8 أو 14 أو 26) مشترك على نفس الخط سيتم تنبيهك بالانتقال لفئة سعة جديدة.
                            </small>
                        </div>
                    </div>

                    <!-- Dates + notes -->
                    <div class="row g-3 mt-3">
                        <div class="col-md-3">
                            <label class="form-label">تاريخ البداية <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtSubStartDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">تاريخ النهاية <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtSubEndDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">ملاحظات</label>
                            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control enhanced-dropdown" placeholder="ملاحظات إضافية"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row g-3 mt-3">
                        <div class="col-md-3">
                            <asp:Button ID="btnSaveSubscription" runat="server" Text="حفظ الاشتراك" CssClass="btn btn-save w-100"
                                OnClick="btnSaveSubscription_Click"
                                OnClientClick="return confirmAndSave();" />
                        </div>
                        <div class="col-md-3">
                            <asp:Button ID="btnClearSubscription" runat="server" Text="مسح" CssClass="btn btn-clear w-100" OnClick="btnClearSubscription_Click" />
                        </div>
                    </div>
                </div>
            </div>

        </div><!-- /.tab-content-custom -->

        <!-- إحصائيات سريعة -->
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

        <asp:HiddenField ID="hdnEmployeeId" runat="server" Value="0" />
        <asp:HiddenField ID="hdnSubscriptionId" runat="server" Value="0" />

        <!-- NEW: threshold confirmation flags -->
        <asp:HiddenField ID="hdnCrossedThreshold" runat="server" Value="false" />
        <asp:HiddenField ID="hdnCapacityAlertMsg" runat="server" Value="" />
    </div>

    <!-- ============ Client-Side JavaScript ============ -->
    <script type="text/javascript">
        // @ts-nocheck
        function getInput(id) { return document.getElementById(id); }
        function getSelect(id) { return document.getElementById(id); }

        function validateNationalId(nationalId) { return nationalId ? /^[23]\d{13}$/.test(nationalId) : false; }
        function validatePhone(phone) { return phone ? /^(010|011|012|015)\d{8}$/.test(phone) : true; }
        function validateEmail(email) { return email ? /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email) : true; }

        function showError(fieldId, message) {
            var field = document.getElementById(fieldId);
            if (!field || !field.parentNode) return;
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
            if (errorSpan) { errorSpan.innerHTML = ""; }
            field.classList.remove("validation-error");
        }

        function validateEmployeeForm() {
            var isValid = true;
            var empName = getInput('<%= txtEmpName.ClientID %>');
            if (empName && empName.value.trim() === "") { showError(empName.id, 'اسم الموظف مطلوب'); isValid = false; }
            else if (empName) { clearError(empName.id); }
            var position = getSelect('<%= ddlEmpPosition.ClientID %>');
            if (position && position.value === "") { showError(position.id, 'الرجاء اختيار الوظيفة'); isValid = false; }
            else if (position) { clearError(position.id); }
            var nationalId = getInput('<%= txtEmpNationalId.ClientID %>');
            if (nationalId && !validateNationalId(nationalId.value.trim())) { showError(nationalId.id, 'الرقم القومي يجب أن يكون 14 رقم ويبدأ بـ 2 أو 3'); isValid = false; }
            else if (nationalId) { clearError(nationalId.id); }
            var email = getInput('<%= txtEmpEmail.ClientID %>');
            if (email && email.value.trim() !== "" && !validateEmail(email.value.trim())) { showError(email.id, 'البريد الإلكتروني غير صحيح'); isValid = false; }
            else if (email) { clearError(email.id); }
            var phone = getInput('<%= txtEmpPhone.ClientID %>');
            if (phone && phone.value.trim() !== "" && !validatePhone(phone.value.trim())) { showError(phone.id, 'رقم الهاتف يجب أن يكون 11 رقم ويبدأ بـ 010/011/012/015'); isValid = false; }
            else if (phone) { clearError(phone.id); }
            return isValid;
        }

        function validateSubscriptionForm() {
            var isValid = true;
            var route = getSelect('<%= ddlRoute.ClientID %>');
            if (route && route.value === "0") { showError(route.id, 'الرجاء اختيار خط السير'); isValid = false; }
            else if (route) { clearError(route.id); }

            var anyChecked = false;
            var cbl = document.getElementById('<%= cblEmployees.ClientID %>');
            if (cbl) {
                var inputs = cbl.querySelectorAll('input[type="checkbox"]');
                for (var i = 0; i < inputs.length; i++) { if (inputs[i].checked) { anyChecked = true; break; } }
            }
            if (!anyChecked) { alert("الرجاء اختيار موظف واحد على الأقل من قائمة الموظفين"); isValid = false; }

            var startDate = getInput('<%= txtSubStartDate.ClientID %>');
            if (startDate && startDate.value === "") { showError(startDate.id, 'تاريخ البداية مطلوب'); isValid = false; }
            else if (startDate) { clearError(startDate.id); }
            var endDate = getInput('<%= txtSubEndDate.ClientID %>');
            if (endDate && endDate.value === "") { showError(endDate.id, 'تاريخ النهاية مطلوب'); isValid = false; }
            else if (endDate) { clearError(endDate.id); }
            if (startDate && endDate && startDate.value !== "" && endDate.value !== "") {
                var s = new Date(startDate.value);
                var e2 = new Date(endDate.value);
                if (e2 <= s) { showError(endDate.id, 'تاريخ النهاية يجب أن يكون بعد تاريخ البداية'); isValid = false; }
            }
            return isValid;
        }

        // ===== NEW: confirm/cancel wrapper for capacity-threshold alert =====
        function confirmAndSave() {
            // 1. Run client-side validation first
            if (!validateSubscriptionForm()) return false;

            // 2. Read flags that the server set on the previous postback
            var crossedField = document.getElementById('<%= hdnCrossedThreshold.ClientID %>');
            var msgField     = document.getElementById('<%= hdnCapacityAlertMsg.ClientID %>');

            var isCrossed = crossedField && crossedField.value === "true";
            var alertMsg  = msgField ? msgField.value : "";

            if (isCrossed) {
                // Show native OK / Cancel dialog
                if (confirm(alertMsg)) {
                    crossedField.value = "confirmed"; // tell the server we approved
                    return true;                      // allow postback -> save happens
                } else {
                    crossedField.value = "false";     // reset, user aborted
                    return false;                     // block postback -> nothing saved
                }
            }

            // No threshold crossed -> normal save
            return true;
        }

        function restrictToNumbers(inputElement, maxLength) {
            if (!inputElement) return;
            inputElement.addEventListener('input', function () {
                this.value = this.value.replace(/[^\d]/g, '');
                if (maxLength && this.value.length > maxLength) this.value = this.value.substring(0, maxLength);
            });
        }

        function activateTab(tabId) {
            var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
            var panes = document.querySelectorAll('.tab-pane-custom');
            for (var i = 0; i < tabs.length; i++) tabs[i].classList.remove('active');
            for (var i = 0; i < panes.length; i++) panes[i].classList.remove('active');
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].getAttribute('data-tab') === tabId) tabs[i].classList.add('active');
            }
            var targetPane = document.getElementById(tabId);
            if (targetPane) targetPane.classList.add('active');
            var activeTabField = getInput('<%= hfActiveTab.ClientID %>');
            if (activeTabField) activeTabField.value = tabId;
        }

        function updateSelectedCount() {
            var lbl = getInput('<%= lblSelectedCount.ClientID %>');
            if (!lbl) return;
            var cbl = document.getElementById('<%= cblEmployees.ClientID %>');
            if (!cbl) return;
            var inputs = cbl.querySelectorAll('input[type="checkbox"]');
            var c = 0;
            for (var i = 0; i < inputs.length; i++) if (inputs[i].checked) c++;
            lbl.textContent = 'عدد المحددين: ' + c;
        }

        document.addEventListener('DOMContentLoaded', function () {
            var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
            var activeTabField = getInput('<%= hfActiveTab.ClientID %>');
            for (var i = 0; i < tabs.length; i++) {
                tabs[i].addEventListener('click', function (e) {
                    e.preventDefault();
                    var attr = this.getAttribute('data-tab');
                    if (attr) activateTab(attr);
                });
            }
            if (activeTabField && activeTabField.value) activateTab(activeTabField.value);

            var btnSaveEmployee = document.getElementById('<%= btnSaveEmployee.ClientID %>');
            if (btnSaveEmployee) btnSaveEmployee.addEventListener('click', function (e) { if (!validateEmployeeForm()) { e.preventDefault(); return false; } });

            // NOTE: btnSaveSubscription is handled by OnClientClick="return confirmAndSave();"
            // Do NOT add a click listener here or validation/confirm will run twice.

            var nationalId = getInput('<%= txtEmpNationalId.ClientID %>');
            if (nationalId) {
                nationalId.addEventListener('blur', function () {
                    if (!validateNationalId(this.value)) showError(this.id, 'الرقم القومي يجب أن يكون 14 رقم ويبدأ بـ 2 أو 3');
                    else clearError(this.id);
                });
                restrictToNumbers(nationalId, 14);
            }

            var phone = getInput('<%= txtEmpPhone.ClientID %>');
            if (phone) {
                phone.addEventListener('blur', function () {
                    if (this.value !== "" && !validatePhone(this.value)) showError(this.id, 'رقم الهاتف غير صحيح (11 رقم يبدأ بـ 010/011/012/015)');
                    else clearError(this.id);
                });
                restrictToNumbers(phone, 11);
            }

            var email = getInput('<%= txtEmpEmail.ClientID %>');
            if (email) email.addEventListener('blur', function () {
                if (this.value !== "" && !validateEmail(this.value)) showError(this.id, 'البريد الإلكتروني غير صحيح');
                else clearError(this.id);
            });

            var empName = getInput('<%= txtEmpName.ClientID %>');
            if (empName) empName.addEventListener('blur', function () {
                if (this.value.trim() === "") showError(this.id, 'اسم الموظف مطلوب');
                else clearError(this.id);
            });

            // live count for checkboxes
            var cbl = document.getElementById('<%= cblEmployees.ClientID %>');
            if (cbl) {
                cbl.addEventListener('change', updateSelectedCount);
                updateSelectedCount();
            }
        });
</script>

</asp:Content>