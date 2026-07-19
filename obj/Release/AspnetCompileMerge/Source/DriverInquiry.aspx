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

        .btn-danger {
            background: #dc3545;
            color: white;
            border: none;
            padding: 5px 10px;
            border-radius: 4px;
        }

        .btn-danger:hover {
            background: #c82333;
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
        
        .search-driver-row {
            display: flex;
            gap: 10px;
            align-items: flex-end;
        }
        
        .search-driver-row .form-group {
            flex: 1;
        }
        
        .search-driver-row button {
            flex: 0 0 100px;
        }
        
        .btn-driver-search {
            background-color: #c8102e;
            color: white;
            border: none;
            height: 45px;
            font-size: 14px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        
        .btn-driver-search:hover {
            background-color: #a50d25;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .search-results-box {
            margin-top: 15px;
            margin-bottom: 15px;
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
        
        @media (max-width: 768px) {
            .search-driver-row {
                flex-direction: column;
            }
            .search-driver-row button {
                flex: 1;
            }
            .stat-card h3 {
                font-size: 24px;
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

        <!-- البحث الموحد -->
        <div class="search-box">
            <h5 class="fw-bold mb-3">
                <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                بحث عن سائق
            </h5>
            
            <div class="row g-3">
                <div class="col-md-8">
                    <label class="form-label">البحث بالاسم</label>
                    <asp:TextBox ID="txtDriverSearch" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        placeholder="أدخل اسم السائق للبحث...">
                    </asp:TextBox>
                </div>
                <div class="col-md-4 d-flex align-items-end">
                    <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-search w-100" OnClick="btnSearch_Click" />
                </div>
            </div>

            <!-- نتائج البحث -->
            <div id="driverSearchResults" runat="server" class="search-results-box" visible="false">
                <div class="table-responsive">
                    <asp:GridView ID="gvDriverSearchResults" runat="server" 
                        CssClass="table table-striped table-bordered table-hover"
                        AutoGenerateColumns="false"
                        EmptyDataText="لا توجد نتائج للبحث"
                        OnRowCommand="gvDriverSearchResults_RowCommand"
                        GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="driver_id" HeaderText="الرقم" />
                            <asp:BoundField DataField="driver_name" HeaderText="اسم السائق" />
                            <asp:BoundField DataField="phone" HeaderText="الهاتف" />
                            <asp:BoundField DataField="national_id" HeaderText="الرقم القومي" />
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
                    </asp:GridView>
                </div>
            </div>
        </div>

       <!-- إضافة/تعديل سائق -->
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
            <asp:TextBox ID="txtNationalId" runat="server" CssClass="form-control enhanced-dropdown" placeholder="الرقم القومي (14 رقم)" MaxLength="14" onkeypress="return isNumber(event)"></asp:TextBox>
        </div>
        
        <div class="col-md-4">
    <label class="form-label">ارقم الهاتف <span class="text-danger">*</span></label>
    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control enhanced-dropdown" placeholder="ارقم الهاتفي (11 رقم)" MaxLength="11" onkeypress="return isNumber(event)"></asp:TextBox>
</div>
    </div>
    
    <div class="row g-3 mt-2">
        <div class="col-md-4">
            <label class="form-label">رقم الرخصة <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtLicenseNumber" runat="server" CssClass="form-control enhanced-dropdown" placeholder="رقم الرخصة (14 رقم)" MaxLength="14" onkeypress="return isNumber(event)"></asp:TextBox>
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
            <label class="form-label">تاريخ بداية الرخصة <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtLicenseStartDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
        </div>
    </div>
    
    <div class="row g-3 mt-2">
        <div class="col-md-4">
            <label class="form-label">تاريخ نهاية الرخصة <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtLicenseEndDate" runat="server" CssClass="form-control enhanced-dropdown" TextMode="Date"></asp:TextBox>
        </div>
        <div class="col-md-8 d-flex align-items-end">
            <asp:Button ID="btnAddDriver" runat="server" Text="إضافة سائق" CssClass="btn btn-save w-100 me-2" OnClick="btnAddDriver_Click" />
            <asp:Button ID="btnUpdateDriver" runat="server" Text="تحديث سائق" CssClass="btn btn-save w-100 me-2" Visible="false" OnClick="btnUpdateDriver_Click" />
            <asp:Button ID="btnCancelDriver" runat="server" Text="إلغاء" CssClass="btn btn-clear w-100" Visible="false" OnClick="btnCancelDriver_Click" />
        </div>
    </div>
</div>
            
            <!-- Hidden fields for IDs -->
            <asp:HiddenField ID="hdnDriverId" runat="server" Value="0" />
            <asp:HiddenField ID="hdnEmployeeId" runat="server" Value="0" />
        </div>

        <!-- قائمة السائقين -->
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
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="الرقم" HeaderText="الرقم" />
                        <asp:BoundField DataField="اسم السائق" HeaderText="اسم السائق" />
                        <asp:BoundField DataField="رقم الرخصة" HeaderText="رقم الرخصة" />
                        <asp:BoundField DataField="رقم الهاتف" HeaderText="رقم الهاتف" />
                        <asp:BoundField DataField="نوع الرخصة" HeaderText="نوع الرخصة" />
                        <asp:BoundField DataField="تاريخ بداية الرخصة" HeaderText="تاريخ بداية الرخصة" />
                        <asp:BoundField DataField="تاريخ نهاية الرخصة" HeaderText="تاريخ نهاية الرخصة" />
                        <asp:BoundField DataField="الرقم القومي" HeaderText="الرقم القومي" />
                        <asp:TemplateField HeaderText="الإجراءات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" 
                                    CommandName="EditDriver" 
                                    CommandArgument='<%# Eval("الرقم") %>'
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
                </asp:GridView>
            </div>
        </div>
        
        <!-- Results Box - Driver Statistics -->
        <div id="resultBox" runat="server" class="result-box" visible="false">
            <div class="section-header">
                <i class="fas fa-chart-line"></i> إحصائيات السائق
            </div>
            
            <div class="grid-container">
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

   
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

   <script type="text/javascript">
       // Allow only numbers
       function isNumber(evt) {
           var charCode = (evt.which) ? evt.which : evt.keyCode;
           if (charCode > 31 && (charCode < 48 || charCode > 57)) {
               return false;
           }
           return true;
       }

       // Enforce maximum length on input
       function enforceMaxLength(input, maxLength) {
           if (input.value.length > maxLength) {
               input.value = input.value.substring(0, maxLength);
           }
       }

       // Enforce only numbers and max length
       function enforceNumberAndMaxLength(input, maxLength) {
           // Remove any non-digit characters
           input.value = input.value.replace(/[^\d]/g, '');
           // Enforce max length
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
            phoneInput.addEventListener('blur', function () {
                if (this.value.length > 0 && this.value.length !== 11) {
                    alert('رقم الهاتف يجب أن يكون 11 رقم بالضبط');
                    this.focus();
                }
            });
        }

        // National ID input - 14 digits
        var nationalIdInput = document.getElementById('<%= txtNationalId.ClientID %>');
        if (nationalIdInput) {
            nationalIdInput.addEventListener('input', function() {
                enforceNumberAndMaxLength(this, 14);
            });
            nationalIdInput.addEventListener('blur', function() {
                enforceNumberAndMaxLength(this, 14);
            });
        }
        
        // License number input - 14 digits
        var licenseInput = document.getElementById('<%= txtLicenseNumber.ClientID %>');
           if (licenseInput) {
               licenseInput.addEventListener('input', function () {
                   enforceNumberAndMaxLength(this, 14);
               });
               licenseInput.addEventListener('blur', function () {
                   enforceNumberAndMaxLength(this, 14);
               });
           }
       });
   </script>
    
    
</asp:Content>