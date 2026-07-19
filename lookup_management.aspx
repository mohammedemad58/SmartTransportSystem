<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="lookup_management.aspx.cs" Inherits="car_WebApplication1.lookup_management" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        .lookup-container {
            padding: 20px;
            direction: rtl;
        }
        
        .lookup-header {
            background: linear-gradient(135deg, #a50d25 0%, #c8102e 100%);
            color: white;
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 25px;
            text-align: center;
        }
        
        .lookup-header h2 {
            margin: 0;
            font-weight: bold;
        }
        
        .lookup-tabs {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-bottom: 25px;
            border-bottom: 2px solid #dee2e6;
            padding-bottom: 10px;
        }
        
        .lookup-tab-btn {
            background: #f8f9fa;
            border: none;
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            font-weight: bold;
            transition: all 0.3s;
            color: #495057;
        }
        
        .lookup-tab-btn:hover {
            background: #e9ecef;
        }
        
        .lookup-tab-btn.active {
            background: #a50d25;
            color: white;
        }
        
        .lookup-section {
            display: none;
            animation: fadeIn 0.3s;
        }
        
        .lookup-section.active {
            display: block;
        }
        
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(10px); }
            to { opacity: 1; transform: translateY(0); }
        }
        
        .card {
            border: 1px solid #dee2e6;
            border-radius: 10px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }
        
        .card-header {
            background: #f8f9fa;
            padding: 15px;
            border-bottom: 1px solid #dee2e6;
            font-weight: bold;
            font-size: 18px;
            border-radius: 10px 10px 0 0;
        }
        
        .card-body {
            padding: 20px;
        }
        
        .form-group {
            margin-bottom: 15px;
        }
        
        .form-label {
            font-weight: bold;
            margin-bottom: 5px;
            display: block;
        }
        
        .required:after {
            content: " *";
            color: #a50d25;
        }
        
        .btn-primary {
            background: #a50d25;
            border: none;
            padding: 8px 20px;
            border-radius: 6px;
            color: white;
            font-weight: bold;
            cursor: pointer;
        }
        
        .btn-primary:hover {
            background: #c8102e;
        }
        
        .btn-secondary {
            background: #6c757d;
            border: none;
            padding: 8px 20px;
            border-radius: 6px;
            color: white;
            cursor: pointer;
        }
        
        .btn-danger {
            background: #dc3545;
            border: none;
            padding: 6px 12px;
            border-radius: 4px;
            color: white;
            cursor: pointer;
            font-size: 12px;
        }
        
        .btn-success {
            background: #28a745;
            border: none;
            padding: 6px 12px;
            border-radius: 4px;
            color: white;
            cursor: pointer;
            font-size: 12px;
        }
        
        .btn-warning {
            background: #ffc107;
            border: none;
            padding: 6px 12px;
            border-radius: 4px;
            color: #000;
            cursor: pointer;
            font-size: 12px;
        }
        
        .lookup-table {
            width: 100%;
            border-collapse: collapse;
        }
        
        .lookup-table th {
            background: #a50d25;
            color: white;
            padding: 12px;
            text-align: center;
        }
        
        .lookup-table td {
            padding: 10px;
            text-align: center;
            border-bottom: 1px solid #dee2e6;
            vertical-align: middle;
        }
        
        .lookup-table tr:hover {
            background: rgba(165, 13, 37, 0.05);
        }
        
        .status-badge {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
        }
        
        .status-active {
            background: #d4edda;
            color: #155724;
        }
        
        .status-inactive {
            background: #f8d7da;
            color: #721c24;
        }
        
        .search-box {
            margin-bottom: 20px;
        }
        
        .search-input {
            width: 100%;
            padding: 8px 12px;
            border: 1px solid #ced4da;
            border-radius: 6px;
        }
        
        .alert {
            padding: 12px;
            border-radius: 6px;
            margin-bottom: 20px;
        }
        
        .alert-success {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        
        .alert-danger {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        
        .inline-input {
            padding: 6px 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            width: 95%;
            min-width: 120px;
        }
        
        .action-cell {
            min-width: 120px;
            white-space: nowrap;
        }
        
        .row {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
        }
        
        .col-md-4, .col-md-6, .col-md-3 {
            flex: 1;
            min-width: 200px;
        }
        
        .w-100 {
            width: 100%;
        }
        
        .pagination-row {
            margin-top: 15px;
            text-align: center;
        }
        
        .card-number-display {
            font-size: 1.1rem;
            font-weight: bold;
            color: #c8102e;
            direction: ltr;
            text-align: left;
            font-family: monospace;
        }
        
        .card-item {
            background-color: white;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 10px;
            border: 1px solid #dee2e6;
            border-right: 3px solid #c8102e;
        }
        
        @media (max-width: 768px) {
            .lookup-table {
                font-size: 12px;
            }
            .inline-input {
                width: 100%;
                min-width: auto;
            }
            .action-cell {
                min-width: auto;
            }
        }
    </style>
    
    <div class="lookup-container">
        
        <div class="lookup-header">
            <h2><i class="fas fa-database"></i> إدارة القوائم الرئيسية</h2>
            <p>إدارة أنواع السيارات، الموديلات، الحالات، وحدات المرور، الكروت، أنواع الوقود، شركات التأمين، وظائف الموظفين، أنواع المخالفات</p>
        </div>
        
        <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
        
        <asp:HiddenField ID="hdnCurrentTab" runat="server" Value="carTypes" />
        
        <div class="lookup-tabs">
            <button type="button" class="lookup-tab-btn" data-tab="carTypes" onclick="showTab('carTypes')">أنواع السيارات</button>
            <button type="button" class="lookup-tab-btn" data-tab="carModels" onclick="showTab('carModels')">نوع هيكل السيارة</button>
            <button type="button" class="lookup-tab-btn" data-tab="carStatus" onclick="showTab('carStatus')">حالات السيارة</button>
            <button type="button" class="lookup-tab-btn" data-tab="trafficUnits" onclick="showTab('trafficUnits')">وحدات المرور</button>
            <button type="button" class="lookup-tab-btn" data-tab="fuelTypes" onclick="showTab('fuelTypes')">أنواع الوقود</button>
            <button type="button" class="lookup-tab-btn" data-tab="fuelPrices" onclick="showTab('fuelPrices')">اسعار الوقود والغاز</button>
            <button type="button" class="lookup-tab-btn" data-tab="licenseTypes" onclick="showTab('licenseTypes')">أنواع الترخيص</button>
            <button type="button" class="lookup-tab-btn" data-tab="insuranceCompanies" onclick="showTab('insuranceCompanies')">شركات التأمين</button>
            <button type="button" class="lookup-tab-btn" data-tab="employeePositions" onclick="showTab('employeePositions')">وظائف الموظفين</button>
            <button type="button" class="lookup-tab-btn" data-tab="violationTypes" onclick="showTab('violationTypes')">أنواع المخالفات</button>
            <button type="button" class="lookup-tab-btn" data-tab="cardsManagement" onclick="showTab('cardsManagement')">إدارة الكروت</button>
        </div>
        
        <!-- Car Types Section -->
        <div id="carTypesSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة نوع سيارة جديد</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label class="form-label required">العلامة التجارية</label>
                                <asp:TextBox ID="txtCarBrand" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label class="form-label required">موديل السيارة</label>
                                <asp:TextBox ID="txtCarTypeName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnAddCarType" runat="server" Text="إضافة نوع سيارة" CssClass="btn-primary w-100" OnClick="BtnAddCarType_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة أنواع السيارات</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchCarType" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchCarType_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvCarTypes" runat="server" AutoGenerateColumns="false" CssClass="lookup-table" 
                        OnRowCommand="GvCarTypes_RowCommand" OnRowEditing="GvCarTypes_RowEditing" OnRowCancelingEdit="GvCarTypes_RowCancelingEdit"
                        OnRowUpdating="GvCarTypes_RowUpdating" DataKeyNames="car_type_id" AllowPaging="true" PageSize="10"
                        OnPageIndexChanging="GvCarTypes_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="car_brand" HeaderText="العلامة التجارية" />
                            <asp:BoundField DataField="car_type_name" HeaderText="موديل السيارة" />
                            <asp:BoundField DataField="full_name" HeaderText="الاسم الكامل" />
                            <asp:TemplateField HeaderText="الحالة">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("car_type_id") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("car_type_id") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditBrand" runat="server" Text='<%# Eval("car_brand") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:TextBox ID="txtEditName" runat="server" Text='<%# Eval("car_type_name") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- Car Models Section -->
        <div id="carModelsSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة نوع هيكل السيارة</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">هيكل السيارة</label>
                                <asp:TextBox ID="txtModelName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddModel" runat="server" Text="إضافة نوع هيكل السيارة" CssClass="btn-primary w-100" OnClick="BtnAddModel_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة هيكل السيارة</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchModel" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchModel_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvModels" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                        OnRowCommand="GvModels_RowCommand" OnRowEditing="GvModels_RowEditing" OnRowCancelingEdit="GvModels_RowCancelingEdit"
                        OnRowUpdating="GvModels_RowUpdating" DataKeyNames="m_id" AllowPaging="true" PageSize="10"
                        OnPageIndexChanging="GvModels_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="model" HeaderText="هيكل السيارة" />
                            <asp:TemplateField HeaderText="الحالة">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("m_id") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("m_id") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditModel" runat="server" Text='<%# Eval("model") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>
        
        <!-- Car Status Section -->
        <div id="carStatusSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة حالة سيارة جديدة</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">حالة السيارة</label>
                                <asp:TextBox ID="txtStatusName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddStatus" runat="server" Text="إضافة حالة" CssClass="btn-primary w-100" OnClick="BtnAddStatus_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة حالات السيارة</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchStatus" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchStatus_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvStatus" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                        OnRowCommand="GvStatus_RowCommand" OnRowEditing="GvStatus_RowEditing" OnRowCancelingEdit="GvStatus_RowCancelingEdit"
                        OnRowUpdating="GvStatus_RowUpdating" DataKeyNames="s_id" AllowPaging="true" PageSize="10"
                        OnPageIndexChanging="GvStatus_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="status" HeaderText="حالة السيارة" />
                            <asp:TemplateField HeaderText="الحالة">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("s_id") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("s_id") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditStatus" runat="server" Text='<%# Eval("status") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>
        
        <!-- Traffic Units Section -->
        <div id="trafficUnitsSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة وحدة مرور جديدة</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">اسم وحدة المرور</label>
                                <asp:TextBox ID="txtTrafficUnitName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddTrafficUnit" runat="server" Text="إضافة وحدة مرور" CssClass="btn-primary w-100" OnClick="BtnAddTrafficUnit_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة وحدات المرور</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchTrafficUnit" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchTrafficUnit_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvTrafficUnits" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                        OnRowCommand="GvTrafficUnits_RowCommand" OnRowEditing="GvTrafficUnits_RowEditing" OnRowCancelingEdit="GvTrafficUnits_RowCancelingEdit"
                        OnRowUpdating="GvTrafficUnits_RowUpdating" DataKeyNames="id_TF" AllowPaging="true" PageSize="10"
                        OnPageIndexChanging="GvTrafficUnits_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="name" HeaderText="وحدة المرور" />
                            <asp:TemplateField HeaderText="الحالة">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("id_TF") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("id_TF") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditTrafficUnit" runat="server" Text='<%# Eval("name") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>
        
        <!-- Fuel Types Section -->
        <div id="fuelTypesSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة نوع وقود جديد</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">نوع الوقود</label>
                                <asp:TextBox ID="txtFuelTypeName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddFuelType" runat="server" Text="إضافة نوع وقود" CssClass="btn-primary w-100" OnClick="BtnAddFuelType_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة أنواع الوقود</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchFuelType" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchFuelType_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvFuelTypes" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                        OnRowCommand="GvFuelTypes_RowCommand" OnRowEditing="GvFuelTypes_RowEditing" OnRowCancelingEdit="GvFuelTypes_RowCancelingEdit"
                        OnRowUpdating="GvFuelTypes_RowUpdating" DataKeyNames="fuel_id" AllowPaging="true" PageSize="10"
                        OnPageIndexChanging="GvFuelTypes_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="type" HeaderText="نوع الوقود" />
                            <asp:TemplateField HeaderText="الحالة">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("fuel_id") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("fuel_id") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditFuelType" runat="server" Text='<%# Eval("type") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>

   <!-- Fuel Prices Section -->
<div id="fuelPricesSection" class="lookup-section">
    <div class="card">
        <div class="card-header">
            <i class="fas fa-gas-pump"></i> أسعار الوقود والغاز الطبيعي
        </div>
        <div class="card-body">
            <div class="alert alert-info">
                <i class="fas fa-info-circle"></i> 
                قم بتحديث أسعار الوقود السائل والغاز الطبيعي هنا. سيتم تطبيق هذه الأسعار تلقائياً عند إدخال تعبئة وقود جديدة.
            </div>
            
            <!-- Fuel (Liquid) Section -->
            <div class="card mb-3">
                <div class="card-header" style="background-color: #fff3cd; border-bottom: 2px solid #ffc107;">
                    <i class="fas fa-oil-can" style="color: #ffc107;"></i> 
                    <strong>أسعار الوقود السائل (بنزين - سولار)</strong>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <asp:GridView ID="gvFuelLiquidPrices" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                            OnRowCommand="GvFuelLiquidPrices_RowCommand" OnRowEditing="GvFuelLiquidPrices_RowEditing" 
                            OnRowCancelingEdit="GvFuelLiquidPrices_RowCancelingEdit" OnRowUpdating="GvFuelLiquidPrices_RowUpdating"
                            DataKeyNames="fuel_type_id" AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="GvFuelLiquidPrices_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="fuel_type_name" HeaderText="نوع الوقود" ItemStyle-Width="40%" ReadOnly="true" />
                                <asp:TemplateField HeaderText="سعر اللتر (ج.ن)" ItemStyle-Width="30%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPricePerLiter" runat="server" 
                                            Text='<%# Convert.ToDecimal(Eval("price_per_liter")).ToString("N2") %>'
                                            CssClass="fw-bold text-primary"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtEditPricePerLiter" runat="server" 
                                            Text='<%# Convert.ToDecimal(Eval("price_per_liter")).ToString("N2") %>' 
                                            CssClass="inline-input" TextMode="Number" step="0.01" min="0"
                                            Width="100%"></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="الإجراءات" ItemStyle-Width="30%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" 
                                            CommandArgument='<%# Eval("fuel_type_id") %>' 
                                            CssClass="btn-warning" CausesValidation="false">
                                            <i class="fas fa-edit"></i> تعديل السعر
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" 
                                            CssClass="btn-success" CausesValidation="false">
                                            <i class="fas fa-save"></i> حفظ
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" 
                                            CssClass="btn-secondary" CausesValidation="false">
                                            <i class="fas fa-times"></i> إلغاء
                                        </asp:LinkButton>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
            
            <!-- Natural Gas Section -->
            <div class="card">
                <div class="card-header" style="background-color: #d1ecf1; border-bottom: 2px solid #17a2b8;">
                    <i class="fas fa-fire" style="color: #17a2b8;"></i> 
                    <strong>أسعار الغاز الطبيعي</strong>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <asp:GridView ID="gvGasPrices" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                            OnRowCommand="GvGasPrices_RowCommand" OnRowEditing="GvGasPrices_RowEditing" 
                            OnRowCancelingEdit="GvGasPrices_RowCancelingEdit" OnRowUpdating="GvGasPrices_RowUpdating"
                            DataKeyNames="fuel_type_id" AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="GvGasPrices_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="fuel_type_name" HeaderText="نوع الوقود" ItemStyle-Width="40%" ReadOnly="true" />
                                <asp:TemplateField HeaderText="سعر المتر المكعب (ج.ن)" ItemStyle-Width="30%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPricePerCubic" runat="server" 
                                            Text='<%# Convert.ToDecimal(Eval("price_per_cubic_meter")).ToString("N2") %>'
                                            CssClass="fw-bold text-success"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtEditPricePerCubic" runat="server" 
                                            Text='<%# Convert.ToDecimal(Eval("price_per_cubic_meter")).ToString("N2") %>' 
                                            CssClass="inline-input" TextMode="Number" step="0.01" min="0"
                                            Width="100%"></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="الإجراءات" ItemStyle-Width="30%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" 
                                            CommandArgument='<%# Eval("fuel_type_id") %>' 
                                            CssClass="btn-warning" CausesValidation="false">
                                            <i class="fas fa-edit"></i> تعديل السعر
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" 
                                            CssClass="btn-success" CausesValidation="false">
                                            <i class="fas fa-save"></i> حفظ
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" 
                                            CssClass="btn-secondary" CausesValidation="false">
                                            <i class="fas fa-times"></i> إلغاء
                                        </asp:LinkButton>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
            
            <div class="mt-3 text-muted">
                <small>
                    <i class="fas fa-info-circle"></i> 
                    <strong>ملاحظة:</strong> يتم تحديث الأسعار هنا وتطبيقها تلقائياً في صفحة إدخال الوقود.
                    <br>
                    <i class="fas fa-oil-can" style="color: #ffc107;"></i> أسعار الوقود السائل للبنزين والسولار
                    <br>
                    <i class="fas fa-fire" style="color: #17a2b8;"></i> أسعار الغاز الطبيعي للمركبات المزودة بغاز
                </small>
            </div>
        </div>
    </div>
</div>
        
        <!-- License Types Section -->
        <div id="licenseTypesSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة نوع ترخيص جديد</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">نوع الترخيص</label>
                                <asp:TextBox ID="txtLicenseTypeName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddLicenseType" runat="server" Text="إضافة نوع ترخيص" CssClass="btn-primary w-100" OnClick="BtnAddLicenseType_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة أنواع الترخيص</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchLicenseType" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchLicenseType_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvLicenseTypes" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                        OnRowCommand="GvLicenseTypes_RowCommand" OnRowEditing="GvLicenseTypes_RowEditing" OnRowCancelingEdit="GvLicenseTypes_RowCancelingEdit"
                        OnRowUpdating="GvLicenseTypes_RowUpdating" DataKeyNames="id_LT" AllowPaging="true" PageSize="10"
                        OnPageIndexChanging="GvLicenseTypes_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="type" HeaderText="نوع الترخيص" />
                            <asp:TemplateField HeaderText="الحالة">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("id_LT") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("id_LT") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditLicenseType" runat="server" Text='<%# Eval("type") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- Insurance Companies Section -->
        <div id="insuranceCompaniesSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة شركة تأمين جديدة</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">اسم شركة التأمين</label>
                                <asp:TextBox ID="txtInsuranceCompanyName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddInsuranceCompany" runat="server" Text="إضافة شركة تأمين" CssClass="btn-primary w-100" OnClick="BtnAddInsuranceCompany_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة شركات التأمين</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchInsuranceCompany" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchInsuranceCompany_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvInsuranceCompanies" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                        OnRowCommand="GvInsuranceCompanies_RowCommand" OnRowEditing="GvInsuranceCompanies_RowEditing" 
                        OnRowCancelingEdit="GvInsuranceCompanies_RowCancelingEdit" OnRowUpdating="GvInsuranceCompanies_RowUpdating"
                        DataKeyNames="id" AllowPaging="true" PageSize="10" OnPageIndexChanging="GvInsuranceCompanies_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="company_name" HeaderText="شركة التأمين" />
                            <asp:TemplateField HeaderText="الحالة">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("id") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("id") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditCompanyName" runat="server" Text='<%# Eval("company_name") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>
        
        <!-- Employee Positions Section -->
        <div id="employeePositionsSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة وظيفة جديدة</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">اسم الوظيفة</label>
                                <asp:TextBox ID="txtPositionName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddPosition" runat="server" Text="إضافة وظيفة" CssClass="btn-primary w-100" OnClick="BtnAddPosition_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة الوظائف</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchPosition" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchPosition_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvPositions" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                        OnRowCommand="GvPositions_RowCommand" OnRowEditing="GvPositions_RowEditing" 
                        OnRowCancelingEdit="GvPositions_RowCancelingEdit" OnRowUpdating="GvPositions_RowUpdating"
                        DataKeyNames="id" AllowPaging="true" PageSize="10" OnPageIndexChanging="GvPositions_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="position_name" HeaderText="اسم الوظيفة" />
                            <asp:TemplateField HeaderText="الحالة">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("id") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("id") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditPositionName" runat="server" Text='<%# Eval("position_name") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>
        
        <!-- Violation Types Section -->
        <div id="violationTypesSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة نوع مخالفة جديد</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">نوع المخالفة</label>
                                <asp:TextBox ID="txtViolationTypeName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddViolationType" runat="server" Text="إضافة نوع مخالفة" CssClass="btn-primary w-100" OnClick="BtnAddViolationType_Click" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">قائمة أنواع المخالفات</div>
                <div class="card-body">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearchViolationType" runat="server" CssClass="search-input" placeholder="بحث..." AutoPostBack="true" OnTextChanged="TxtSearchViolationType_TextChanged"></asp:TextBox>
                    </div>
                    <asp:GridView ID="gvViolationTypes" runat="server" AutoGenerateColumns="false" CssClass="lookup-table"
                        OnRowCommand="GvViolationTypes_RowCommand" OnRowEditing="GvViolationTypes_RowEditing" 
                        OnRowCancelingEdit="GvViolationTypes_RowCancelingEdit" OnRowUpdating="GvViolationTypes_RowUpdating"
                        DataKeyNames="id" AllowPaging="true" PageSize="10" OnPageIndexChanging="GvViolationTypes_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="RowNum" HeaderText="#" ItemStyle-Width="50px" ReadOnly="true" />
                            <asp:BoundField DataField="type_name" HeaderText="نوع المخالفة" />
                            <asp:TemplateField HeaderText="الحالة" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <span class='status-badge <%# Convert.ToBoolean(Eval("is_active")) ? "status-active" : "status-inactive" %>'>
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="الإجراءات" ItemStyle-CssClass="action-cell" ItemStyle-Width="150px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("id") %>' CssClass="btn-warning" CausesValidation="false">تعديل</asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("id") %>' CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn-danger" : "btn-success" %>' CausesValidation="false">
                                        <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtEditTypeName" runat="server" Text='<%# Eval("type_name") %>' CssClass="inline-input"></asp:TextBox>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn-success" CausesValidation="false">حفظ</asp:LinkButton>
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn-secondary" CausesValidation="false">إلغاء</asp:LinkButton>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-row" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>
        </div>
        
        <!-- Cards Management Section -->
        <div id="cardsManagementSection" class="lookup-section">
            <div class="card">
                <div class="card-header">إضافة كارت جديد</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label required">رقم الكارت</label>
                                <asp:TextBox ID="txtNewCardNumber" runat="server" CssClass="form-control" placeholder="أدخل رقم الكارت" dir="ltr"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <asp:Button ID="btnAddCard" runat="server" Text="إضافة كارت" CssClass="btn-primary w-100" OnClick="BtnAddCard_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-header">قائمة الكروت الموجودة</div>
                <div class="card-body">
                    <asp:Repeater ID="rptCards" runat="server" OnItemCommand="RptCards_ItemCommand">
                        <ItemTemplate>
                            <div class="card-item">
                                <div style="display: flex; justify-content: space-between; align-items: center;">
                                    <div>
                                        <span class="card-number-display"><%# Eval("card_number") %></span>
                                    </div>
                                    <div>
                                        <asp:LinkButton ID="btnDeleteCard" runat="server" 
                                            CommandName="DeleteCard" 
                                            CommandArgument='<%# Eval("card_id") %>'
                                            CssClass="btn-danger"
                                            OnClientClick="return confirm('هل أنت متأكد من حذف هذا الكارت؟');">
                                            <i class="fas fa-trash"></i> حذف
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlNoCards" runat="server" CssClass="text-center text-muted" Visible="false">
                        <p>لا توجد كروت مضافة</p>
                    </asp:Panel>
                </div>
            </div>
        </div>
        
    </div>
    
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    
    <script type="text/javascript">
        function showTab(tabName) {
            var sections = document.getElementsByClassName('lookup-section');
            for (var i = 0; i < sections.length; i++) {
                sections[i].classList.remove('active');
            }
            
            var tabs = document.getElementsByClassName('lookup-tab-btn');
            for (var i = 0; i < tabs.length; i++) {
                tabs[i].classList.remove('active');
            }
            
            var selectedSection = document.getElementById(tabName + 'Section');
            if (selectedSection) {
                selectedSection.classList.add('active');
            }
            
            if (event && event.target) {
                event.target.classList.add('active');
            }
            
            var hdnCurrentTab = document.getElementById('<%= hdnCurrentTab.ClientID %>');
            if (hdnCurrentTab) {
                hdnCurrentTab.value = tabName;
            }
        }

        function restoreLastTab() {
            var hdnCurrentTab = document.getElementById('<%= hdnCurrentTab.ClientID %>');
            if (hdnCurrentTab && hdnCurrentTab.value) {
                var lastTab = hdnCurrentTab.value;
                var buttons = document.getElementsByClassName('lookup-tab-btn');
                for (var i = 0; i < buttons.length; i++) {
                    if (buttons[i].getAttribute('data-tab') === lastTab) {
                        buttons[i].click();
                        break;
                    }
                }
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            restoreLastTab();
        });
    </script>
    
</asp:Content>