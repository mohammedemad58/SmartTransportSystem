<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
    CodeBehind="work_order.aspx.cs" Inherits="car_WebApplication1.work_order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Print Styles */
        @media print {
            /* Hide buttons and search sections when printing */
            .btn-search, .search-box, .action-btn, .btn-print, .btn-save, .btn-clear,
            .modal, .modal-footer, .modal-header, .modal-dialog,
            .section-header i, .action-btn, .btn, button {
                display: none !important;
            }
            
            /* Remove background colors for printing */
            .result-box, .search-box, .container, body {
                background-color: white !important;
                border: none !important;
                padding: 0 !important;
                margin: 0 !important;
            }
            
            /* Ensure text is black for printing */
            * {
                color: black !important;
                background: transparent !important;
                box-shadow: none !important;
            }
            
            /* Show borders for tables */
            table, th, td {
                border: 1px solid #ccc !important;
                border-collapse: collapse !important;
            }
            
            /* Hide the search results section */
            #searchResults {
                display: none !important;
            }
            
            /* Adjust spacing for print */
            .result-box {
                margin: 0 !important;
                padding: 10px !important;
            }
            
            .page-title {
                font-size: 18pt !important;
                margin-bottom: 10px !important;
            }
            
            .section-header {
                background: none !important;
                border-bottom: 2px solid #000 !important;
                margin-top: 15px !important;
            }
        }
        
        /* ===== RTL ARABIC SUPPORT ===== */
        * { direction: rtl; text-align: right; }
        body, html, .container, .container-fluid { direction: rtl; }
        .row { margin-right: 0; margin-left: 0; }
        .col-md-2, .col-md-3, .col-md-4, .col-md-9, .col-md-12 { float: right; }
        .text-center { text-align: center; }
        .form-label { text-align: right; display: block; }
        .modal-header .btn-close { margin: 0; }
        
        .page-title { color: #000; font-weight: bold; font-size: 28px; margin-bottom: 25px; }
        .search-box { border: 2px solid #000; border-radius: 10px; padding: 20px; background-color: #fff; margin-bottom: 25px; }
        .form-label { font-weight: bold; margin-bottom: 8px; color: #333; font-size: 14px; }
        .btn-search { background-color: #c8102e; color: #fff; font-weight: bold; height: 45px; font-size: 16px; }
        .btn-search:hover { background-color: #a50d25; color: #fff; }
        .result-box { border: 2px solid #000; border-radius: 10px; padding: 25px; margin-top: 20px; background-color: #f9f9f9; }
        .dropdown-custom { text-align: right; direction: rtl; height: 45px; font-size: 15px; }
        .form-control { text-align: right; height: 45px; font-size: 15px; }
        .row-spacing { margin-bottom: 20px; }
        
        .enhanced-dropdown { background: #fff; border: 2px solid #dee2e6; border-radius: 8px; transition: all 0.3s ease; }
        .enhanced-dropdown:hover { border-color: #c8102e; }
        .enhanced-dropdown:focus { border-color: #c8102e; box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1); }
        
        .section-header { background: linear-gradient(90deg, #c8102e, #ffffff); padding: 10px 15px; border-radius: 8px; margin-bottom: 20px; font-weight: bold; color: #000; font-size: 18px; }
        .section-header i { margin-left: 10px; color: #c8102e; }
        
        .subscribers-container { background: #f8f9fa; padding: 15px; border-radius: 8px; border: 2px solid #dee2e6; margin-top: 10px; }
        .selected-items { min-height: 60px; max-height: 200px; overflow-y: auto; background: white; border: 2px solid #dee2e6; border-radius: 8px; padding: 15px; margin-top: 15px; }
        
        .subscriber-tag { display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 8px 15px; margin: 5px; border-radius: 30px; font-size: 14px; border: none; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .employee-tag { background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); }
        .subscriber-tag .remove-item { color: #fff; margin-right: 8px; cursor: pointer; font-weight: bold; opacity: 0.8; transition: opacity 0.2s; }
        .subscriber-tag .remove-item:hover { opacity: 1; color: #ffcccc; }
        
        .table-header { background-color: #c8102e; color: white; }
        .table-row:hover { background-color: #f5f5f5; }
        .table-alt-row { background-color: #f9f9f9; }
        
        .action-btn { height: 50px; font-size: 16px; font-weight: bold; border-radius: 8px; transition: all 0.3s ease; }
        .action-btn:hover { transform: translateY(-2px); box-shadow: 0 4px 8px rgba(0,0,0,0.2); }
        .btn-save { background: #28a745; color: white; border: none; }
        .btn-print { background: #17a2b8; color: white; border: none; }
        .btn-clear { background: #ffc107; color: #333; border: none; }
        
        .form-control[readonly] { background-color: #e9ecef; opacity: 1; cursor: default; }
        
        .km-warning {
            display: none;
            font-size: 12px;
            margin-top: 5px;
            padding: 5px;
            border-radius: 4px;
        }
        .km-warning.show {
            display: block;
        }
        .km-warning.warning {
            background-color: #fff3cd;
            color: #856404;
            border: 1px solid #ffeeba;
        }
        .km-warning.error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        .input-error {
            border-color: #dc3545 !important;
        }
        
        .employees-textbox {
            background-color: #fff;
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

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css"/>

    <div class="container mt-4" dir="rtl">

        <div class="text-center">
            <h2 class="page-title">
                <i class="fas fa-clipboard-list" style="color: #c8102e; margin-left: 10px;"></i>
                بيانات امر الشغل
            </h2>
        </div>

        <!-- رسائل التنبيه -->
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <!-- Hidden field to remember active tab across postbacks -->
        <asp:HiddenField ID="hfActiveTab" runat="server" Value="tabSearch" />

        <!-- ===== TABS ===== -->
        <div class="nav-tabs-custom" id="mainTabs">
            <div class="nav-item">
                <button class="nav-link active" data-tab="tabSearch">
                    <i class="fas fa-search"></i> البحث
                </button>
            </div>
            <div class="nav-item">
                <button class="nav-link" data-tab="tabForm">
                    <i class="fas fa-clipboard-list"></i> امر شغل جديد
                </button>
            </div>
        </div>

        <div class="tab-content-custom">

            <!-- ===== TAB 1: SEARCH ===== -->
            <div class="tab-pane-custom active" id="tabSearch">
                <!-- Search Section -->
                <div class="search-box">
                    <h5 class="fw-bold mb-3">
                        <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                        البحث عن أوامر الشغل
                    </h5>
                    <div class="row g-3">
                        <div class="col-md-4">
                            <label class="form-label">رقم السيارة</label>
                            <asp:TextBox ID="txtSearchCarNumber" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل رقم السيارة للبحث"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">اسم السائق</label>
                            <asp:TextBox ID="txtSearchDriverName" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل اسم السائق للبحث"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">التاريخ</label>
                            <asp:TextBox ID="txtSearchDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-2 d-flex align-items-end">
                            <asp:Button ID="btnSearchOperations" runat="server" Text="بحث" CssClass="btn btn-search w-100" OnClick="btnSearchOperations_Click" />
                        </div>
                    </div>
                </div>

                <!-- Search Results -->
                <div id="searchResults" runat="server" class="result-box" visible="false">
                    <h5 class="fw-bold mb-3"><i class="fas fa-list" style="color: #c8102e; margin-left: 8px;"></i>نتائج البحث</h5>
                    <div class="table-responsive">
                        <asp:GridView ID="gvSearchResults" runat="server" CssClass="table table-striped table-bordered table-hover"
                            AutoGenerateColumns="false" EmptyDataText="لا توجد نتائج للبحث" OnRowCommand="gvSearchResults_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="mission_id" HeaderText="رقم الأمر" />
                                <asp:BoundField DataField="car_number" HeaderText="رقم السيارة" />
                                <asp:BoundField DataField="mission_date" HeaderText="التاريخ" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="driver_name" HeaderText="اسم السائق" />
                                <asp:BoundField DataField="mission_type" HeaderText="نوع المأمورية" />
                                <asp:BoundField DataField="mission_details" HeaderText="التفاصيل" />
                                <asp:BoundField DataField="kilometer_start" HeaderText="بداية (كم)" />
                                <asp:BoundField DataField="kilometer_end" HeaderText="نهاية (كم)" />
                                <asp:BoundField DataField="distance_display" HeaderText="المسافة" />
                                <asp:TemplateField HeaderText="إجراءات">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnSelect" runat="server" CommandName="SelectOperation" CommandArgument='<%# Eval("mission_id") %>' CssClass="btn btn-sm btn-info">
                                            <i class="fas fa-eye"></i> عرض
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
            </div>

            <!-- ===== TAB 2: WORK ORDER FORM ===== -->
            <div class="tab-pane-custom" id="tabForm">
                <!-- Main Form -->
                <div class="result-box">
                    <!-- Basic Data Section -->
                    <div class="section-header"><i class="fas fa-info-circle"></i> البيانات الأساسية</div>
                    <div class="row g-3 row-spacing">
                        <div class="col-md-4">
                            <label class="form-label">رقم العربية <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlCarNumber" runat="server" CssClass="form-control dropdown-custom enhanced-dropdown"
                                AppendDataBoundItems="false" DataTextField="full_plate_number" DataValueField="car_number_id"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlCarNumber_SelectedIndexChanged"
                                onchange="storeCurrentTab();">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">نوع السيارة</label>
                            <asp:DropDownList ID="ddlCarType" runat="server" CssClass="form-control dropdown-custom enhanced-dropdown"
                                DataTextField="car_type_name" DataValueField="car_type_id" Enabled="false">
                                <asp:ListItem Text="-- اختر نوع السيارة --" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">اسم السائق <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlDriverName" runat="server" CssClass="form-control dropdown-custom enhanced-dropdown"
                                DataTextField="driver_name" DataValueField="driver_id" AppendDataBoundItems="true"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlDriverName_SelectedIndexChanged"
                                onchange="storeCurrentTab();">
                                <asp:ListItem Text="-- اختر اسم السائق --" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Vehicle Details -->
                    <div class="row g-3 row-spacing">
                        <div class="col-md-4">
                            <label class="form-label">ماركة السيارة</label>
                            <asp:TextBox ID="txtCarBrand" runat="server" CssClass="form-control enhanced-dropdown" ReadOnly="true" placeholder="--"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">سنة الصنع</label>
                            <asp:TextBox ID="txtModelYear" runat="server" CssClass="form-control enhanced-dropdown" ReadOnly="true" placeholder="--"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">نوع الوقود</label>
                            <asp:TextBox ID="txtFuelType" runat="server" CssClass="form-control enhanced-dropdown" ReadOnly="true" placeholder="--"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Mission Data Section -->
                    <div class="section-header mt-4"><i class="fas fa-route"></i> بيانات المهمة</div>
                    <div class="row g-3 row-spacing">
                        <div class="col-md-4">
                            <label class="form-label">نوع المأمورية <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlMissionType" runat="server" CssClass="form-control dropdown-custom enhanced-dropdown"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlMissionType_SelectedIndexChanged"
                                onchange="storeCurrentTab();">
                                <asp:ListItem Text="-- اختر نوع المأمورية --" Value="0" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="مأمورية" Value="mission"></asp:ListItem>
                                <asp:ListItem Text="خط" Value="route"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-4" id="routeDropdownContainer" runat="server" visible="false">
                            <label class="form-label">خط السير <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlRoute" runat="server" CssClass="form-control dropdown-custom enhanced-dropdown"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlRoute_SelectedIndexChanged"
                                DataTextField="display_text" DataValueField="route_id"
                                onchange="storeCurrentTab();">
                                <asp:ListItem Text="-- اختر خط السير --" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <div id="routeDescriptionContainer" runat="server" class="mt-2 p-2 bg-light border rounded" visible="false">
                                <asp:Label ID="lblRouteDescription" runat="server" CssClass="text-muted"></asp:Label>
                            </div>
                        </div>

                        <div class="col-md-4" id="missionTextContainer" runat="server" visible="false">
                            <label class="form-label">نص المأمورية <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtMissionText" runat="server" CssClass="form-control enhanced-dropdown" placeholder="أدخل نص المأمورية" MaxLength="500"></asp:TextBox>
                        </div>

                        <!-- Subscribers Container - visible for "خط" -->
                        <div class="col-md-4" id="subscribersContainer" runat="server" visible="false">
                            <label class="form-label">أسماء المشتركين</label>
                            <div class="subscribers-container">
                                <div class="row">
                                    <div class="col-md-9">
                                        <select id="ddlSubscriberSelector" class="form-control enhanced-dropdown">
                                            <option value="">-- اختر مشترك --</option>
                                        </select>
                                    </div>
                                    <div class="col-md-3">
                                        <button type="button" class="btn btn-success w-100" onclick="addSelectedSubscriber()">
                                            <i class="fas fa-plus"></i> إضافة
                                        </button>
                                    </div>
                                </div>
                                <div id="selectedSubscribersList" class="selected-items"></div>
                            </div>
                            <asp:HiddenField ID="hdnSelectedSubscribers" runat="server" />
                            <asp:HiddenField ID="hdnSubscribersJson" runat="server" />
                        </div>

                        <!-- Employees Container - visible for "مأمورية" -->
                        <div class="col-md-4" id="employeesContainer" runat="server" visible="false">
                            <label class="form-label">الموظفين <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtEmployees" runat="server" CssClass="form-control enhanced-dropdown employees-textbox" 
                                placeholder="أدخل أسماء الموظفين (افصل بينهم بفاصلة)" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            <div class="invalid-feedback">الرجاء إدخال أسماء الموظفين</div>
                            <small class="form-text text-muted">مثال: أحمد محمد، علي حسين، محمود سامي</small>
                            <asp:HiddenField ID="hdnSelectedEmployees" runat="server" />
                        </div>
                    </div>

                    <!-- Time and Distance Section -->
                    <div class="section-header mt-4"><i class="fas fa-clock"></i> بيانات الوقت والمسافة</div>
                    <div class="row g-3 row-spacing">
                        <div class="col-md-3">
                            <label class="form-label">بداية المأمورية</label>
                            <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control time-input enhanced-dropdown" 
                                TextMode="DateTimeLocal" ReadOnly="true" BackColor="#F8F9FA"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">وقت العودة</label>
                            <asp:TextBox ID="txtReturnTime" runat="server" CssClass="form-control time-input enhanced-dropdown" 
                                TextMode="DateTimeLocal" ReadOnly="true" BackColor="#F8F9FA"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">عداد البداية (كم)</label>
                            <asp:TextBox ID="txtStartKM" runat="server" CssClass="form-control enhanced-dropdown" 
                                placeholder="أدخل عداد البداية" TextMode="Number" step="0.1" min="0" 
                                ReadOnly="true" BackColor="#F8F9FA"></asp:TextBox>
                            <div id="kmWarning" class="km-warning"></div>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">عداد العودة (كم)</label>
                            <asp:TextBox ID="txtEndKM" runat="server" CssClass="form-control enhanced-dropdown" 
                                placeholder="أدخل عداد العودة" TextMode="Number" step="0.1" min="0"
                                ReadOnly="true" BackColor="#F8F9FA"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Personnel Section -->
                    <div class="section-header mt-4"><i class="fas fa-users"></i> بيانات الموظفين</div>
                    <div class="row g-3 row-spacing">
                        <div class="col-md-4">
                            <label class="form-label">اسم كاتب الجراج</label>
                            <asp:DropDownList ID="ddlGarageWriter" runat="server" CssClass="form-control dropdown-custom enhanced-dropdown"
                                DataTextField="employee_name" DataValueField="employee_id"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlGarageWriter_SelectedIndexChanged"
                                onchange="storeCurrentTab();">
                                <asp:ListItem Text="-- اختر كاتب الجراج --" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">اسم ملاحظ المركبات</label>
                            <asp:DropDownList ID="ddlVehicleObserver" runat="server" CssClass="form-control dropdown-custom enhanced-dropdown"
                                DataTextField="employee_name" DataValueField="employee_id"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlVehicleObserver_SelectedIndexChanged"
                                onchange="storeCurrentTab();">
                                <asp:ListItem Text="-- اختر ملاحظ المركبات --" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">اسم مدير التشغيل</label>
                            <asp:DropDownList ID="ddlOperationManager" runat="server" CssClass="form-control dropdown-custom enhanced-dropdown"
                                DataTextField="employee_name" DataValueField="employee_id"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlOperationManager_SelectedIndexChanged"
                                onchange="storeCurrentTab();">
                                <asp:ListItem Text="-- اختر مدير التشغيل --" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Hidden Fields -->
                    <asp:HiddenField ID="hdnLastKilometer" runat="server" Value="0" />

                    <!-- Action Buttons -->
                    <div class="row mt-5">
                        <div class="col-md-4 text-center">
                            <asp:Button ID="btnSave" runat="server" Text="تسجيل" CssClass="btn action-btn btn-save w-100" 
                                OnClick="btnSave_Click" OnClientClick="return validateAllFields();" />
                        </div>
                        <div class="col-md-4 text-center">
                            <asp:Button ID="btnPrint" runat="server" Text="طباعة" CssClass="btn action-btn btn-print w-100" 
                                OnClick="btnPrint_Click" />
                        </div>
                        <div class="col-md-4 text-center">
                            <asp:Button ID="btnClear" runat="server" Text="امر شغل جديد" CssClass="btn action-btn btn-clear w-100" 
                                OnClick="btnClear_Click" OnClientClick="return confirm('هل أنت متأكد من مسح جميع البيانات؟');" />
                        </div>
                    </div>
                </div>
            </div>

        </div><!-- /.tab-content-custom -->
    </div>

    <script type="text/javascript">
        let selectedSubscribers = [];

        // ==================== SUBSCRIBERS FUNCTIONS ====================

        function loadSubscribersDropdown() {
            const dropdown = document.getElementById('ddlSubscriberSelector');
            if (!dropdown) {
                console.error('Subscriber dropdown not found');
                return;
            }

            const currentSelection = dropdown.value;
            dropdown.innerHTML = '<option value="">-- اختر مشترك --</option>';

            if (typeof routeSubscribers !== 'undefined' && routeSubscribers !== null && routeSubscribers.length > 0) {
                console.log('Loading', routeSubscribers.length, 'subscribers for this route');

                routeSubscribers.forEach(function (sub) {
                    if (sub && sub.id && sub.name) {
                        var option = document.createElement('option');
                        option.value = sub.id;
                        option.text = sub.name;
                        dropdown.appendChild(option);
                    }
                });

                if (currentSelection) {
                    dropdown.value = currentSelection;
                }
            } else {
                const hiddenField = document.getElementById('<%= hdnSubscribersJson.ClientID %>');
                if (hiddenField && hiddenField.value) {
                    try {
                        var subscribersList = JSON.parse(hiddenField.value);
                        subscribersList.forEach(function (sub) {
                            if (sub && sub.id && sub.name) {
                                var option = document.createElement('option');
                                option.value = sub.id;
                                option.text = sub.name;
                                dropdown.appendChild(option);
                            }
                        });
                    } catch (e) { console.error(e); }
                }
            }
        }

        function addSelectedSubscriber() {
            const dropdown = document.getElementById('ddlSubscriberSelector');
            if (!dropdown) return;

            const selectedId = dropdown.value;
            const selectedText = dropdown.options[dropdown.selectedIndex]?.text;

            if (!selectedId) {
                alert('الرجاء اختيار مشترك');
                return;
            }

            if (selectedSubscribers.some(s => s.id === selectedId)) {
                alert('هذا المشترك مضاف بالفعل');
                return;
            }

            selectedSubscribers.push({
                id: selectedId,
                name: selectedText
            });

            updateSubscribersList();
            updateHiddenField();
            dropdown.value = '';
        }

        function removeSubscriber(id) {
            selectedSubscribers = selectedSubscribers.filter(s => s.id !== id);
            updateSubscribersList();
            updateHiddenField();
        }

        function updateSubscribersList() {
            const listContainer = document.getElementById('selectedSubscribersList');
            if (!listContainer) return;

            listContainer.innerHTML = '';

            if (selectedSubscribers.length === 0) {
                listContainer.innerHTML = '<p class="text-muted text-center p-3">لا يوجد مشتركين مختارين</p>';
                return;
            }

            selectedSubscribers.forEach(sub => {
                const tag = document.createElement('span');
                tag.className = 'subscriber-tag';
                tag.innerHTML = sub.name + ' <span class="remove-item" onclick="removeSubscriber(\'' + sub.id + '\')">×</span>';
                listContainer.appendChild(tag);
            });
        }

        function updateHiddenField() {
            const ids = selectedSubscribers.map(s => s.id).join(',');
            const hiddenField = document.getElementById('<%= hdnSelectedSubscribers.ClientID %>');
            if (hiddenField) {
                hiddenField.value = ids;
                console.log('Updated hidden field with:', ids);
            }
        }

        // ==================== KILOMETER VALIDATION ====================

        function validateStartKM() {
            const startKM = document.getElementById('<%= txtStartKM.ClientID %>');
            const warningSpan = document.getElementById('kmWarning');
            const lastKMHidden = document.getElementById('<%= hdnLastKilometer.ClientID %>');

            if (!startKM || !startKM.value) {
                if (warningSpan) {
                    warningSpan.style.display = 'none';
                    warningSpan.innerHTML = '';
                }
                return true;
            }

            if (lastKMHidden && lastKMHidden.value && parseFloat(lastKMHidden.value) > 0) {
                const startValue = parseFloat(startKM.value);
                const lastValue = parseFloat(lastKMHidden.value);

                if (startValue < lastValue) {
                    warningSpan.style.display = 'block';
                    warningSpan.className = 'km-warning error show';
                    warningSpan.innerHTML = '⚠️ تحذير: عداد البداية (' + startValue + ' كم) أقل من آخر عداد مسجل (' + lastValue + ' كم)';
                    startKM.style.borderColor = '#dc3545';
                    return false;
                } else if (startValue > lastValue + 5000) {
                    warningSpan.style.display = 'block';
                    warningSpan.className = 'km-warning warning show';
                    warningSpan.innerHTML = '⚠️ تنبيه: عداد البداية (' + startValue + ' كم) أعلى بكثير من آخر عداد مسجل (' + lastValue + ' كم)';
                    startKM.style.borderColor = '#ffc107';
                    return true;
                } else {
                    warningSpan.style.display = 'none';
                    warningSpan.innerHTML = '';
                    startKM.style.borderColor = '';
                    return true;
                }
            }
            return true;
        }

        // ==================== VALIDATION FUNCTIONS ====================

        function validateAllFields() {
            const missionType = document.getElementById('<%= ddlMissionType.ClientID %>').value;
            const carNumber = document.getElementById('<%= ddlCarNumber.ClientID %>').value;
            const driver = document.getElementById('<%= ddlDriverName.ClientID %>').value;
            const employeesText = document.getElementById('<%= txtEmployees.ClientID %>');
            
            if (!carNumber || carNumber === '0') { alert('الرجاء اختيار رقم العربية'); return false; }
            if (!missionType || missionType === '0') { alert('الرجاء اختيار نوع المأمورية'); return false; }
            if (!driver || driver === '0') { alert('الرجاء اختيار اسم السائق'); return false; }
            
            if (missionType === 'route') {
                const route = document.getElementById('<%= ddlRoute.ClientID %>').value;
                if (!route || route === '0') { alert('الرجاء اختيار خط السير'); return false; }
                if (selectedSubscribers.length === 0) { alert('الرجاء اختيار مشترك واحد على الأقل'); return false; }
            } else if (missionType === 'mission') {
                const missionText = document.getElementById('<%= txtMissionText.ClientID %>').value;
                if (!missionText.trim()) { alert('الرجاء إدخال نص المأمورية'); return false; }
                if (employeesText && !employeesText.value.trim()) { alert('الرجاء إدخال الموظفين'); return false; }
            }
            
            return true;
        }

        // ==================== TAB SWITCHING ====================

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

        // ==================== INITIALIZATION ====================

        document.addEventListener('DOMContentLoaded', function () {
            loadSubscribersDropdown();
            
            // Start KM validation is optional now - only validate if field has value
            const startKM = document.getElementById('<%= txtStartKM.ClientID %>');
            if (startKM) {
                startKM.addEventListener('blur', function() {
                    if (startKM.value) {
                        validateStartKM();
                    }
                });
                startKM.addEventListener('input', function() {
                    const warningSpan = document.getElementById('kmWarning');
                    if (warningSpan) {
                        warningSpan.style.display = 'none';
                    }
                    this.style.borderColor = '';
                });
            }

            // Tab click handlers
            var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link');
            var activeTabField = document.getElementById('<%= hfActiveTab.ClientID %>');

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
                activateTab('tabSearch');
            }

            // Store current tab before any postback via form submit
            var forms = document.getElementsByTagName('form');
            if (forms.length > 0) {
                forms[0].addEventListener('submit', function() {
                    storeCurrentTab();
                });
            }

            // Also store when dropdowns change (for AutoPostBack)
            var dropdowns = document.querySelectorAll('select[onchange*="storeCurrentTab"]');
            dropdowns.forEach(function(ddl) {
                ddl.addEventListener('change', function() {
                    storeCurrentTab();
                });
            });
        });

        if (typeof Sys !== 'undefined' && Sys.Application) {
            Sys.Application.add_load(function () {
                loadSubscribersDropdown();
                updateSubscribersList();
            });
        }
    </script>

    <!-- Success Modal -->
    <div class="modal fade" id="successModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-success text-white">
                    <h5 class="modal-title">تم الحفظ بنجاح</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body" id="successMessageContent"></div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    <button type="button" class="btn btn-primary" onclick="printCurrentWorkOrder()">طباعة</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showSuccessMessage(message) {
            document.getElementById('successMessageContent').innerHTML = message;
            new bootstrap.Modal(document.getElementById('successModal')).show();
        }
        function printCurrentWorkOrder() {
            bootstrap.Modal.getInstance(document.getElementById('successModal')).hide();
            window.print();
        }
    </script>

</asp:Content>