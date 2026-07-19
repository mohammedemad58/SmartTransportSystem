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

        <!-- قسم البحث عن المخالفات -->
        <div class="search-box">
            <h5 class="fw-bold mb-3">
                <i class="fas fa-search" style="color: #c8102e; margin-left: 8px;"></i>
                البحث عن المخالفات
            </h5>
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label">رقم السيارة</label>
                    <asp:TextBox ID="txtSearchCar" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        placeholder="أدخل رقم السيارة"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <label class="form-label">اسم السائق</label>
                    <asp:TextBox ID="txtSearchDriver" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        placeholder="أدخل اسم السائق"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <label class="form-label">من تاريخ</label>
                    <asp:TextBox ID="txtFromDate" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        TextMode="Date"></asp:TextBox>
                </div>
                <div class="col-md-2">
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
                    <asp:Button ID="btnReset" runat="server" 
                        Text="إعادة تعيين" 
                        CssClass="btn btn-secondary w-100 ms-2"
                        OnClick="btnReset_Click" />
                </div>
            </div>
        </div>

        <!-- قسم تسجيل مخالفة جديدة -->
        <div class="result-box mb-4">
            <div class="section-header">
                <i class="fas fa-plus-circle"></i> تسجيل مخالفة جديدة
            </div>

            <div class="row g-3">
                <!-- رقم العربية - Dropdown -->
                <div class="col-md-4">
                    <label class="form-label">رقم العربية <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlCarNumber" runat="server"
                        CssClass="form-control enhanced-dropdown"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlCarNumber_SelectedIndexChanged"
                        DataTextField="car_number_display"
                        DataValueField="car_number_id">
                        <asp:ListItem Text="-- اختر رقم العربية --" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- اسم السائق - Dropdown -->
                <div class="col-md-4">
                    <label class="form-label">اسم السائق <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlDriverName" runat="server"
                        CssClass="form-control enhanced-dropdown"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlDriverName_SelectedIndexChanged"
                        DataTextField="driver_name"
                        DataValueField="driver_id">
                        <asp:ListItem Text="-- اختر اسم السائق --" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- المهمة Dropdown -->
                <div class="col-md-4">
                    <label class="form-label">المهمة <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlMission" runat="server"
                        CssClass="form-control enhanced-dropdown"
                        DataTextField="mission_display"
                        DataValueField="mission_id">
                        <asp:ListItem Text="-- اختر المهمة --" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="row g-3 mt-3">
                <!-- نوع المخالفة -->
                <div class="col-md-4">
                    <label class="form-label">نوع المخالفة <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlViolationType" runat="server"
                        CssClass="form-control enhanced-dropdown">
                        <asp:ListItem Text="-- اختر نوع المخالفة --" Value="0"></asp:ListItem>
                        <asp:ListItem Text="سرعة زائدة" Value="سرعة زائدة"></asp:ListItem>
                        <asp:ListItem Text="تجاوز إشارة حمراء" Value="تجاوز إشارة حمراء"></asp:ListItem>
                        <asp:ListItem Text="سير عكس الاتجاه" Value="سير عكس الاتجاه"></asp:ListItem>
                        <asp:ListItem Text="استخدام الهاتف أثناء القيادة" Value="استخدام الهاتف أثناء القيادة"></asp:ListItem>
                        <asp:ListItem Text="عدم ربط حزام الأمان" Value="عدم ربط حزام الأمان"></asp:ListItem>
                        <asp:ListItem Text="انبعاثات عالية" Value="انبعاثات عالية"></asp:ListItem>
                        <asp:ListItem Text="تجاوز الخط الأبيض" Value="تجاوز الخط الأبيض"></asp:ListItem>
                        <asp:ListItem Text="تجاوز الوزن المسموح" Value="تجاوز الوزن المسموح"></asp:ListItem>
                        <asp:ListItem Text="وقوف خاطئ" Value="وقوف خاطئ"></asp:ListItem>
                        <asp:ListItem Text="أخرى" Value="أخرى"></asp:ListItem>
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
                <div class="col-md-3">
                    <label class="form-label">تاريخ المخالفة <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtViolationDate" runat="server" 
                        CssClass="form-control enhanced-dropdown" 
                        TextMode="Date"></asp:TextBox>
                </div>

                <!-- الموقع -->
                <div class="col-md-3">
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
                    OnRowCommand="gvViolations_RowCommand"
                    AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvViolations_PageIndexChanging"
                    GridLines="None"
                    DataKeyNames="violation_id">
                    <Columns>
                        <asp:BoundField DataField="violation_id" HeaderText="رقم المخالفة" />
                        <asp:BoundField DataField="car_number" HeaderText="رقم السيارة" />
                        <asp:BoundField DataField="driver_name" HeaderText="اسم السائق" />
                        <asp:BoundField DataField="mission_date" HeaderText="تاريخ المهمة" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="violation_type" HeaderText="نوع المخالفة" />
                        <asp:BoundField DataField="violation_amount" HeaderText="القيمة" DataFormatString="{0:N2} ج.م" />
                        <asp:BoundField DataField="violation_date" HeaderText="تاريخ المخالفة" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="location" HeaderText="الموقع" />
                        <asp:TemplateField HeaderText="الإجراءات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" 
                                    CommandName="EditViolation" 
                                    CommandArgument='<%# Eval("violation_id") %>'
                                    CssClass="action-btn btn-edit"
                                    ToolTip="تعديل">
                                    <i class="fas fa-edit"></i> تعديل
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" 
                                    CommandName="DeleteViolation" 
                                    CommandArgument='<%# Eval("violation_id") %>'
                                    CssClass="action-btn btn-delete"
                                    OnClientClick="return confirm('هل أنت متأكد من حذف هذه المخالفة؟');"
                                    ToolTip="حذف">
                                    <i class="fas fa-trash"></i> حذف
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

        <!-- إحصائيات سريعة -->
        <div class="row mt-4">
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

    <!-- Hidden field for editing -->
    <asp:HiddenField ID="hdnViolationId" runat="server" Value="0" />

</asp:Content>