<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
    CodeBehind="work_order.aspx.cs" Inherits="car_WebApplication1.work_order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* ===== RTL ARABIC SUPPORT - ADDED ===== */
        * {
            direction: rtl;
            text-align: right;
        }
        
        body, html, .container, .container-fluid {
            direction: rtl;
        }
        
        .row {
            margin-right: 0;
            margin-left: 0;
        }
        
        .col-md-2, .col-md-3, .col-md-4, .col-md-9, .col-md-12 {
            float: right;
        }
        
        .text-center {
            text-align: center;
        }
        
        .form-label {
            text-align: right;
            display: block;
        }
        
        .modal-header .btn-close {
            margin: 0;
        }
        
        .input-group:not(.has-validation) > :not(:last-child):not(.dropdown-toggle):not(.dropdown-menu),
        .input-group:not(.has-validation) > .dropdown-toggle:nth-last-child(n+3) {
            border-top-left-radius: 0;
            border-bottom-left-radius: 0;
            border-top-right-radius: 0.375rem;
            border-bottom-right-radius: 0.375rem;
        }
        
        .input-group > :not(:first-child):not(.dropdown-menu):not(.valid-tooltip):not(.valid-feedback):not(.invalid-tooltip):not(.invalid-feedback) {
            margin-right: -1px;
            border-top-right-radius: 0;
            border-bottom-right-radius: 0;
            border-top-left-radius: 0.375rem;
            border-bottom-left-radius: 0.375rem;
        }
        /* ===== END RTL ARABIC SUPPORT ===== */
        
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
        .result-box {
            border: 2px solid #000;
            border-radius: 10px;
            padding: 25px;
            margin-top: 20px;
            background-color: #f9f9f9;
        }
        .dropdown-custom {
            text-align: right;
            direction: rtl;
            height: 45px;
            font-size: 15px;
        }
        .form-control {
            text-align: right;
            height: 45px;
            font-size: 15px;
        }
        .row-spacing {
            margin-bottom: 20px;
        }
        
        /* Car Number Container Styles */
        .car-number-container {
            background: linear-gradient(to bottom, #f8f9fa, #ffffff);
            border: 2px solid #c8102e;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 15px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }
        
        .car-number-label {
            font-size: 16px;
            font-weight: bold;
            color: #c8102e;
            margin-bottom: 15px;
            display: flex;
            align-items: center;
            gap: 10px;
        }
        
        .car-number-label i {
            font-size: 24px;
            color: #c8102e;
        }
        
        .car-number-dropdown {
            width: 100%;
        }
        
        .car-number-dropdown select {
            height: 55px !important;
            font-size: 18px !important;
            font-weight: bold !important;
            border: 2px solid #c8102e !important;
            border-radius: 10px !important;
            background-color: #fff !important;
            padding: 0 15px !important;
            cursor: pointer;
            transition: all 0.3s ease;
        }
        
        .car-number-dropdown select:hover {
            border-color: #a50d25 !important;
            box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1);
        }
        
        .car-number-dropdown select:focus {
            border-color: #c8102e !important;
            box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.25);
            outline: none;
        }
        
        .car-number-dropdown option {
            font-size: 16px;
            padding: 12px;
        }
        
        .car-number-helper {
            margin-top: 8px;
            font-size: 13px;
            color: #666;
            display: flex;
            align-items: center;
            gap: 5px;
        }
        
        .car-number-helper i {
            color: #c8102e;
            font-size: 14px;
        }
        
        /* Enhanced dropdowns for other fields */
        .enhanced-dropdown {
            background: #fff;
            border: 2px solid #dee2e6;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        
        .enhanced-dropdown:hover {
            border-color: #c8102e;
        }
        
        .enhanced-dropdown:focus {
            border-color: #c8102e;
            box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1);
        }
        
        /* Section headers */
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
        
        /* Subscribers Container Styles */
        .subscribers-container {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            border: 2px solid #dee2e6;
            margin-top: 10px;
        }
        
        .selected-subscribers {
            min-height: 60px;
            max-height: 200px;
            overflow-y: auto;
            background: white;
            border: 2px solid #dee2e6;
            border-radius: 8px;
            padding: 15px;
            margin-top: 15px;
        }
        
        .subscriber-tag {
            display: inline-block;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 8px 15px;
            margin: 5px;
            border-radius: 30px;
            font-size: 14px;
            border: none;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        
        .subscriber-tag .remove-subscriber {
            color: #fff;
            margin-right: 8px;
            cursor: pointer;
            font-weight: bold;
            opacity: 0.8;
            transition: opacity 0.2s;
        }
        
        .subscriber-tag .remove-subscriber:hover {
            opacity: 1;
            color: #ffcccc;
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
        
        /* Action buttons */
        .action-btn {
            height: 50px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        
        .action-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .btn-save {
            background: #a50d25;
            color: white;
            border: none;
        }
        
        .btn-print {
            background:  #a50d25;
            color: white;
            border: none;
        }
        
        .btn-clear {
            background:  #a50d25;
            color: white;
            border: none;
        }
        
        /* Read-only textbox styling */
        .form-control[readonly] {
            background-color: #e9ecef;
            opacity: 1;
            cursor: default;
        }
        /* AutoComplete Styles */
.autocomplete-container {
    position: relative;
    width: 100%;
}

.autocomplete-suggestions {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    max-height: 250px;
    overflow-y: auto;
    background: white;
    border: 1px solid #ced4da;
    border-top: none;
    border-radius: 0 0 8px 8px;
    z-index: 1000;
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
    display: none;
}

.autocomplete-suggestion {
    padding: 10px 15px;
    cursor: pointer;
    border-bottom: 1px solid #f0f0f0;
    text-align: right;
    transition: background-color 0.2s;
}

.autocomplete-suggestion:hover {
    background-color: #f8f9fa;
}

.autocomplete-suggestion.selected {
    background-color: #c8102e;
    color: white;
}

.autocomplete-suggestion .suggestion-subtext {
    font-size: 12px;
    color: #666;
    margin-right: 10px;
}

.autocomplete-suggestion.selected .suggestion-subtext {
    color: #ffcccc;
}

.autocomplete-loading {
    position: absolute;
    left: 10px;
    top: 12px;
    color: #999;
    display: none;
}

/* Hidden field for storing selected ID */
.hidden-id-field {
    display: none;
}
    </style>

    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css"/>

    <div class="container mt-4" dir="rtl">

        <!-- عنوان الصفحة -->
        <div class="text-center">
            <h2 class="page-title">
                <i class="fas fa-clipboard-list" style="color: #c8102e; margin-left: 10px;"></i>
                بيانات امر الشغل
            </h2>
        </div>
        
        <!-- قسم البحث -->
        <div class="search-box">
            <h5 class="fw-bold mb-3">
                <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                البحث عن أوامر الشغل
            </h5>
            <div class="row g-3">
                <div class="col-md-4">
                    <label class="form-label">رقم السيارة</label>
                    <div class="input-group">
                        <asp:TextBox ID="txtSearchCarNumber" runat="server" 
                            CssClass="form-control enhanced-dropdown" 
                            placeholder="أدخل رقم السيارة للبحث">
                        </asp:TextBox>
                        <button class="btn btn-outline-secondary" type="button" onclick="clearSearchCarNumber()">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
                
                <div class="col-md-4">
                    <label class="form-label">اسم السائق</label>
                    <div class="input-group">
                        <asp:TextBox ID="txtSearchDriverName" runat="server" 
                            CssClass="form-control enhanced-dropdown" 
                            placeholder="أدخل اسم السائق للبحث">
                        </asp:TextBox>
                        <button class="btn btn-outline-secondary" type="button" onclick="clearSearchDriverName()">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
                
                <div class="col-md-2">
                    <label class="form-label">التاريخ</label>
                    <asp:TextBox ID="txtSearchDate" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        TextMode="Date"
                        placeholder="اختر التاريخ">
                    </asp:TextBox>
                </div>
                
                <div class="col-md-2 d-flex align-items-end">
                    <asp:Button ID="btnSearchOperations" runat="server" 
                        Text="بحث" 
                        CssClass="btn btn-search w-100"
                        OnClick="btnSearchOperations_Click" />
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
      <asp:GridView ID="gvSearchResults" runat="server" 
    CssClass="table table-striped table-bordered table-hover"
    AutoGenerateColumns="false"
    EmptyDataText="لا توجد نتائج للبحث"
    OnRowCommand="gvSearchResults_RowCommand"
    GridLines="None">
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
                <asp:LinkButton ID="btnSelect" runat="server" 
                    CommandName="SelectOperation" 
                    CommandArgument='<%# Eval("mission_id") %>'
                    CssClass="btn btn-sm btn-info">
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
    
   
    
</div>  <!-- This closes the searchResults div -->
            </div>
      

        <!-- نتائج البيانات -->
        <div class="result-box">
            <!-- القسم الأول: البيانات الأساسية -->
            <div class="section-header">
                <i class="fas fa-info-circle"></i> البيانات الأساسية
            </div>
            
            <div class="row g-3 row-spacing">
                <!-- رقم العربية - Dropdown with AutoPostBack -->
                <div class="col-md-4">
                    <label class="form-label">رقم العربية <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlCarNumber" runat="server" 
                        CssClass="form-control dropdown-custom enhanced-dropdown"
                        AppendDataBoundItems="false"
                        DataTextField="full_plate_number" 
                        DataValueField="car_number_id"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlCarNumber_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>

                <!-- نوع السيارة - Read Only Dropdown -->
                <div class="col-md-4">
                    <label class="form-label">نوع السيارة</label>
                    <asp:DropDownList ID="ddlCarType" runat="server"
                        CssClass="form-control dropdown-custom enhanced-dropdown"
                        DataTextField="car_type_name"
                        DataValueField="car_type_id"
                        Enabled="false">
                        <asp:ListItem Text="-- اختر نوع السيارة --" Value="0" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- اسم السائق -->
                <div class="col-md-4">
                    <label class="form-label">اسم السائق <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlDriverName" runat="server"
                        CssClass="form-control dropdown-custom enhanced-dropdown"
                        DataTextField="driver_name"
                        DataValueField="driver_id"
                        AppendDataBoundItems="true">
                        <asp:ListItem Text="-- اختر اسم السائق --" Value="0" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                    <div class="invalid-feedback">الرجاء اختيار اسم السائق</div>
                </div>
            </div>

            <!-- NEW ROW: Additional Vehicle Details -->
            <div class="row g-3 row-spacing">
                <!-- ماركة السيارة - Read Only TextBox -->
                <div class="col-md-4">
                    <label class="form-label">ماركة السيارة</label>
                    <asp:TextBox ID="txtCarBrand" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        ReadOnly="true"
                        placeholder="--">
                    </asp:TextBox>
                </div>

                <!-- سنة الصنع - Read Only TextBox -->
                <div class="col-md-4">
                    <label class="form-label">سنة الصنع</label>
                    <asp:TextBox ID="txtModelYear" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        ReadOnly="true"
                        placeholder="--">
                    </asp:TextBox>
                </div>

                <!-- نوع الوقود - Read Only TextBox -->
                <div class="col-md-4">
                    <label class="form-label">نوع الوقود</label>
                    <asp:TextBox ID="txtFuelType" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        ReadOnly="true"
                        placeholder="--">
                    </asp:TextBox>
                </div>
            </div>

            <!-- القسم الثاني: بيانات المهمة -->
            <div class="section-header mt-4">
                <i class="fas fa-route"></i> بيانات المهمة
            </div>

            <div class="row g-3 row-spacing">
                <!-- نوع المأمورية Dropdown -->
                <div class="col-md-4">
                    <label class="form-label">نوع المأمورية <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlMissionType" runat="server"
                        CssClass="form-control dropdown-custom enhanced-dropdown"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlMissionType_SelectedIndexChanged">
                        <asp:ListItem Text="-- اختر نوع المأمورية --" Value="0" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="مأمورية" Value="mission"></asp:ListItem>
                        <asp:ListItem Text="خط" Value="route"></asp:ListItem>
                    </asp:DropDownList>
                    <div class="invalid-feedback">الرجاء اختيار نوع المأمورية</div>
                </div>

               <!-- خط السير Dropdown (visible when "خط" is selected) -->
<div class="col-md-4" id="routeDropdownContainer" runat="server" visible="false">
    <label class="form-label">خط السير <span class="text-danger">*</span></label>
    <asp:DropDownList ID="ddlRoute" runat="server"
        CssClass="form-control dropdown-custom enhanced-dropdown"
        AutoPostBack="true"
        OnSelectedIndexChanged="ddlRoute_SelectedIndexChanged"
        DataTextField="display_text"
        DataValueField="route_id">
        <asp:ListItem Text="-- اختر خط السير --" Value="0" Selected="True"></asp:ListItem>
    </asp:DropDownList>
    <div class="invalid-feedback">الرجاء اختيار خط السير</div>
    
    <!-- Description display area -->
    <div id="routeDescriptionContainer" runat="server" class="mt-2 p-2 bg-light border rounded" visible="false">
        <asp:Label ID="lblRouteDescription" runat="server" CssClass="text-muted"></asp:Label>
    </div>
</div>

                <!-- نص المأمورية TextBox (visible when "مأمورية" is selected) -->
                <div class="col-md-4" id="missionTextContainer" runat="server" visible="false">
                    <label class="form-label">نص المأمورية <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtMissionText" runat="server"
                        CssClass="form-control enhanced-dropdown"
                        placeholder="أدخل نص المأمورية"
                        MaxLength="500">
                    </asp:TextBox>
                    <div class="invalid-feedback">الرجاء إدخال نص المأمورية</div>
                </div>

                <!-- المشتركين -->
                <div class="col-md-4">
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
                        
                        <div id="selectedSubscribersList" class="selected-subscribers">
                            <!-- المشتركين المختارين سيظهرون هنا -->
                        </div>
                    </div>
                    
                    <!-- Hidden fields -->
                    <asp:HiddenField ID="hdnSelectedSubscribers" runat="server" />
                    <asp:HiddenField ID="hdnSubscribersJson" runat="server" />
                </div>
            </div>

            <!-- القسم الثالث: بيانات الوقت والمسافة -->
            <div class="section-header mt-4">
                <i class="fas fa-clock"></i> بيانات الوقت والمسافة
            </div>
            
            <div class="row g-3 row-spacing">
                <div class="col-md-3">
                    <label class="form-label">بداية المأمورية</label>
                    <asp:TextBox ID="txtStartTime" runat="server"
                        CssClass="form-control time-input enhanced-dropdown"
                        placeholder="أدخل بداية المأمورية"
                        TextMode="DateTimeLocal"></asp:TextBox>
                    <div class="invalid-feedback">الرجاء إدخال بداية المأمورية</div>
                </div>

                <div class="col-md-3">
                    <label class="form-label">وقت العودة</label>
                    <asp:TextBox ID="txtReturnTime" runat="server"
                        CssClass="form-control time-input enhanced-dropdown"
                        placeholder="أدخل وقت العودة"
                        TextMode="DateTimeLocal"></asp:TextBox>
                    <div class="invalid-feedback">الرجاء إدخال وقت العودة</div>
                </div>

                <div class="col-md-3">
                    <label class="form-label">عداد البداية (كم)</label>
                    <asp:TextBox ID="txtStartKM" runat="server"
                        CssClass="form-control enhanced-dropdown"
                        placeholder="أدخل عداد البداية"
                        TextMode="Number" step="0.1" min="0"></asp:TextBox>
                    <div class="invalid-feedback">الرجاء إدخال عداد البداية</div>
                </div>

                <div class="col-md-3">
                    <label class="form-label">عداد العودة (كم)</label>
                    <asp:TextBox ID="txtEndKM" runat="server"
                        CssClass="form-control enhanced-dropdown"
                        placeholder="أدخل عداد العودة"
                        TextMode="Number" step="0.1" min="0"></asp:TextBox>
                    <div class="invalid-feedback">الرجاء إدخال عداد العودة</div>
                </div>
            </div>

            <!-- القسم الرابع: بيانات الموظفين -->
            <div class="section-header mt-4">
                <i class="fas fa-users"></i> بيانات الموظفين
            </div>
            
            <div class="row g-3 row-spacing">
   <!-- اسم كاتب الجراج Dropdown -->
<div class="col-md-4">
    <label class="form-label">اسم كاتب الجراج</label>
    <asp:DropDownList ID="ddlGarageWriter" runat="server"
        CssClass="form-control dropdown-custom enhanced-dropdown"
        DataTextField="employee_name"
        DataValueField="employee_id">
        <asp:ListItem Text="-- اختر كاتب الجراج --" Value="0" Selected="True"></asp:ListItem>
    </asp:DropDownList>
</div>

<!-- اسم ملاحظ المركبات Dropdown -->
<div class="col-md-4">
    <label class="form-label">اسم ملاحظ المركبات</label>
    <asp:DropDownList ID="ddlVehicleObserver" runat="server"
        CssClass="form-control dropdown-custom enhanced-dropdown"
        DataTextField="employee_name"
        DataValueField="employee_id">
        <asp:ListItem Text="-- اختر ملاحظ المركبات --" Value="0" Selected="True"></asp:ListItem>
    </asp:DropDownList>
</div>

<!-- اسم مدير إدارة التشغيل Dropdown -->
<div class="col-md-4">
    <label class="form-label">اسم مدير التشغيل</label>
    <asp:DropDownList ID="ddlOperationManager" runat="server"
        CssClass="form-control dropdown-custom enhanced-dropdown"
        DataTextField="employee_name"
        DataValueField="employee_id">
        <asp:ListItem Text="-- اختر مدير التشغيل --" Value="0" Selected="True"></asp:ListItem>
    </asp:DropDownList>
</div>
            </div>

            <!-- أزرار الإجراءات -->
            <div class="row mt-5">
                <div class="col-md-4 text-center">
                    <asp:Button ID="btnSave" runat="server"
                        Text="تسجيل"
                        CssClass="btn action-btn btn-save w-100" 
                        OnClick="btnSave_Click"
                        OnClientClick="return validateAllFields();" />
                </div>
                <div class="col-md-4 text-center">
                    <asp:Button ID="btnPrint" runat="server"
                        Text="طباعة"
                        CssClass="btn action-btn btn-print w-100" 
                        OnClientClick="printWorkOrder(); return false;" />
                </div>
                <div class="col-md-4 text-center">
                    <asp:Button ID="btnClear" runat="server"
                        Text="امر شغل جديد"
                        CssClass="btn action-btn btn-clear w-100" 
                        OnClick="btnClear_Click"
                        OnClientClick="return confirm('هل أنت متأكد من مسح جميع البيانات؟');" />
                </div>
            </div>
        </div>
   

    <!-- JavaScript -->
    <script type="text/javascript">
        let selectedSubscribers = [];

        function loadSubscribersDropdown() {
            const dropdown = document.getElementById('ddlSubscriberSelector');
            if (!dropdown) {
                console.error('Subscriber dropdown not found');
                return;
            }

            // Store current selection if any
            const currentSelection = dropdown.value;

            // Clear dropdown
            dropdown.innerHTML = '<option value="">-- اختر مشترك --</option>';

            // Check if subscribersList exists and has data
            if (typeof subscribersList !== 'undefined' && subscribersList !== null && subscribersList.length > 0) {
                console.log('Loading', subscribersList.length, 'subscribers into dropdown');

                subscribersList.forEach(function (sub) {
                    if (sub && sub.id && sub.name) {
                        var option = document.createElement('option');
                        option.value = sub.id;
                        option.text = sub.name;
                        dropdown.appendChild(option);
                    }
                });

                // Try to restore previous selection
                if (currentSelection) {
                    dropdown.value = currentSelection;
                }
            } else {
                console.warn('subscribersList is empty or undefined');
                // Try to get data from hidden field if available
                const hiddenField = document.getElementById('<%= hdnSubscribersJson.ClientID %>');
                if (hiddenField && hiddenField.value) {
                    try {
                        subscribersList = JSON.parse(hiddenField.value);
                        loadSubscribersDropdown(); // Recursive call
                    } catch (e) {
                        console.error('Failed to parse subscribers from hidden field', e);
                    }
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
                tag.innerHTML = `
                    ${sub.name} 
                    <span class="remove-subscriber" onclick="removeSubscriber('${sub.id}')">×</span>
                `;
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

        function validateAllFields() {
            // Check if car number is selected
            const carNumberDropdown = document.getElementById('<%= ddlCarNumber.ClientID %>');
            if (!carNumberDropdown || !carNumberDropdown.value || carNumberDropdown.value === '0') {
                alert('الرجاء اختيار رقم العربية');
                return false;
            }

            // Check if mission type is selected
            const missionTypeDropdown = document.getElementById('<%= ddlMissionType.ClientID %>');
            if (!missionTypeDropdown || !missionTypeDropdown.value || missionTypeDropdown.value === '0') {
                alert('الرجاء اختيار نوع المأمورية');
                return false;
            }

            // Check if driver is selected
            const driverDropdown = document.getElementById('<%= ddlDriverName.ClientID %>');
            if (!driverDropdown || !driverDropdown.value || driverDropdown.value === '0') {
                alert('الرجاء اختيار اسم السائق');
                return false;
            }

            // Check mission type specific fields
            const missionType = document.getElementById('<%= ddlMissionType.ClientID %>').value;
            if (missionType === 'route') {
                // Check if route is selected
                const routeDropdown = document.getElementById('<%= ddlRoute.ClientID %>');
                if (!routeDropdown || !routeDropdown.value || routeDropdown.value === '0') {
                    alert('الرجاء اختيار خط السير');
                    return false;
                }
            } else if (missionType === 'mission') {
                // Check if mission text is entered
                const missionText = document.getElementById('<%= txtMissionText.ClientID %>');
                if (!missionText || !missionText.value.trim()) {
                    alert('الرجاء إدخال نص المأمورية');
                    return false;
                }
            }

            // التحقق من اختيار مشتركين
            if (selectedSubscribers.length === 0) {
                alert('الرجاء اختيار مشترك واحد على الأقل');
                return false;
            }

            // Check time fields
            const startTime = document.getElementById('<%= txtStartTime.ClientID %>');
            const returnTime = document.getElementById('<%= txtReturnTime.ClientID %>');
            
            if (!startTime.value) {
                alert('الرجاء إدخال بداية المأمورية');
                return false;
            }
            
            if (!returnTime.value) {
                alert('الرجاء إدخال وقت العودة');
                return false;
            }

            // Check KM fields
            const startKM = document.getElementById('<%= txtStartKM.ClientID %>');
            const endKM = document.getElementById('<%= txtEndKM.ClientID %>');

            if (!startKM.value) {
                alert('الرجاء إدخال عداد البداية');
                return false;
            }

            if (!endKM.value) {
                alert('الرجاء إدخال عداد العودة');
                return false;
            }

            return true;
        }

        function printWorkOrder() {
            window.print();
            return false;
        }

        // Initialize on page load
        document.addEventListener('DOMContentLoaded', function () {
            console.log('DOM fully loaded - initializing subscribers');
            loadSubscribersDropdown();
            updateSubscribersList();
        });

        // Handle postbacks (like when selecting car number)
        if (typeof Sys !== 'undefined' && Sys.Application) {
            Sys.Application.add_load(function () {
                console.log('Postback detected - reloading subscribers');
                loadSubscribersDropdown();
                updateSubscribersList();
            });
        }
    </script>

    <!-- Success Message Modal -->
    <div class="modal fade" id="successModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-success text-white">
                    <h5 class="modal-title">تم الحفظ بنجاح</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body" id="successMessageContent">
                    <!-- Success message will be inserted here -->
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إغلاق</button>
                    <button type="button" class="btn btn-primary" onclick="printCurrentWorkOrder()">طباعة</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showSuccessMessage(message) {
            var contentElement = document.getElementById('successMessageContent');
            if (contentElement) {
                contentElement.innerHTML = message;
            }
            var successModal = new bootstrap.Modal(document.getElementById('successModal'));
            successModal.show();
        }

        function printCurrentWorkOrder() {
            var modal = bootstrap.Modal.getInstance(document.getElementById('successModal'));
            if (modal) modal.hide();
            window.print();
        }
    </script>

</asp:Content>