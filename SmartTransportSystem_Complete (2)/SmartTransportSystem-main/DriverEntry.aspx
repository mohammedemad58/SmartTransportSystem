<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="DriverEntry.aspx.cs"
    Inherits="car_WebApplication1.DriverEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
        }

        .entry-box {
            border: 2px solid #000;
            border-radius: 14px;
            padding: 25px;
            background-color: #fff;
            margin-bottom: 20px;
        }

        .form-label {
            font-weight: bold;
            margin-bottom: 5px;
            display: block;
        }

        .form-control {
            width: 100%;
            padding: 10px 15px;
            border: 2px solid #dee2e6;
            border-radius: 10px;
            font-size: 16px;
            font-family: inherit;
            background-color: white;
            transition: all 0.3s;
            margin-bottom: 15px;
        }

        .form-control:focus {
            outline: none;
            border-color: #ce1126;
            box-shadow: 0 0 0 3px rgba(206, 17, 38, 0.25);
        }

        .form-control:hover {
            border-color: #999;
        }

        .btn-save {
             background-color: #c8102e;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 50px;
            border: none;
            border-radius: 10px;
            font-size: 18px;
            transition: all 0.3s;
        }

        .btn-save:hover {
            background-color: #218838;
        }

        .btn-clear {
             background-color: #6c757d;
             color: #fff;
            font-weight: bold;
            height: 50px;
            border: none;
            border-radius: 10px;
            font-size: 18px;
            transition: all 0.3s;
        }

        .btn-clear:hover {
            background-color: #e0a800;
        }

        .section-header {
            background: linear-gradient(90deg, #ce1126, #ffffff);
            padding: 10px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-weight: bold;
            color: #000;
        }

        .required:after {
            content: " *";
            color: #ce1126;
            font-weight: bold;
        }

        .checkbox-container {
            display: flex;
            align-items: center;
            margin-bottom: 15px;
        }

        .checkbox-container input {
            width: 20px;
            height: 20px;
            margin-left: 10px;
            cursor: pointer;
        }

        .checkbox-container label {
            font-weight: bold;
            cursor: pointer;
            margin-bottom: 0;
        }

        .routes-container {
            border: 2px solid #dee2e6;
            border-radius: 10px;
            padding: 15px;
            max-height: 200px;
            overflow-y: auto;
            background-color: #f8f9fa;
            margin-bottom: 15px;
        }

        .route-item {
            padding: 8px 12px;
            margin: 5px 0;
            background-color: white;
            border: 1px solid #dee2e6;
            border-radius: 5px;
            transition: all 0.2s;
        }

        .route-item:hover {
            background-color: #e9ecef;
            border-color: #ce1126;
        }

        .route-item input[type="checkbox"] {
            margin-left: 10px;
            transform: scale(1.1);
            cursor: pointer;
        }

        .route-item label {
            font-weight: normal;
            cursor: pointer;
            margin-bottom: 0;
        }

        .readonly-field {
            background-color: #e9ecef;
        }

        .info-text {
            color: #6c757d;
            font-size: 0.9rem;
            margin-top: -10px;
            margin-bottom: 15px;
        }

        @media (max-width: 768px) {
            .btn-save, .btn-clear {
                width: 100%;
                margin-bottom: 10px;
            }
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <!-- العنوان -->
        <div class="text-center mb-4">
            <h2 class="page-title">إدخال بيانات السائقين</h2>
        </div>

        <!-- رسالة خطأ -->
        <asp:Label ID="lblMessage" runat="server" 
            CssClass="alert alert-danger text-center d-block" 
            Visible="false">
        </asp:Label>

        <!-- رسالة نجاح -->
        <asp:Label ID="lblSuccess" runat="server" 
            CssClass="alert alert-success text-center d-block" 
            Visible="false">
        </asp:Label>

        <!-- نموذج الإدخال -->
        <div class="entry-box">
            <div class="section-header">
                <i class="fas fa-user-plus"></i> بيانات السائق الجديد
            </div>

            <!-- الصف الأول: البيانات الأساسية -->
            <div class="row">
                <div class="col-md-6">
                    <label class="form-label required">اسم السائق</label>
                    <asp:TextBox ID="txtDriverName" runat="server" 
                        CssClass="form-control" 
                        placeholder="أدخل اسم السائق">
                    </asp:TextBox>
                </div>

                <div class="col-md-6">
                    <label class="form-label required">كود السائق</label>
                    <asp:TextBox ID="txtDriverCode" runat="server" 
                        CssClass="form-control" 
                        placeholder="مثال: DRV001">
                    </asp:TextBox>
                </div>
            </div>

            <!-- الصف الثاني: بيانات الرخصة -->
            <div class="row">
                <div class="col-md-6">
                    <label class="form-label required">رقم الرخصة</label>
                    <asp:TextBox ID="txtLicenseNumber" runat="server" 
                        CssClass="form-control" 
                        placeholder="أدخل رقم الرخصة">
                    </asp:TextBox>
                </div>

                <div class="col-md-6">
                    <label class="form-label required">رقم الهاتف</label>
                    <asp:TextBox ID="txtPhone" runat="server" 
                        CssClass="form-control" 
                        placeholder="مثال: 01234567890">
                    </asp:TextBox>
                </div>
            </div>

            <!-- الصف الثالث: نوع الرخصة والتواريخ -->
            <div class="row">
                <div class="col-md-4">
                    <label class="form-label required">نوع الرخصة</label>
                    <asp:DropDownList ID="ddlLicenseType" runat="server" CssClass="form-control">
                        <asp:ListItem Text="-- اختر نوع الرخصة --" Value="" />
                        <asp:ListItem Text="رخصة درجة أولى" Value="رخصة درجة أولى" />
                        <asp:ListItem Text="رخصة درجة ثانية" Value="رخصة درجة ثانية" />
                        <asp:ListItem Text="رخصة درجة ثالثة" Value="رخصة درجة ثالثة" />
                        <asp:ListItem Text="رخصة مهنية" Value="رخصة مهنية" />
                    </asp:DropDownList>
                </div>

                <div class="col-md-4">
                    <label class="form-label required">تاريخ بداية الرخصة</label>
                    <asp:TextBox ID="txtLicenseStartDate" runat="server" 
                        CssClass="form-control" 
                        TextMode="Date">
                    </asp:TextBox>
                </div>

                <div class="col-md-4">
                    <label class="form-label required">تاريخ نهاية الرخصة</label>
                    <asp:TextBox ID="txtLicenseEndDate" runat="server" 
                        CssClass="form-control" 
                        TextMode="Date">
                    </asp:TextBox>
                </div>
            </div>

            <!-- الصف الرابع: المهام والمسافات -->
            <div class="row">
                <div class="col-md-3">
                    <label class="form-label">عدد المهام</label>
                    <asp:TextBox ID="txtWriteSubCount" runat="server" 
                        CssClass="form-control" 
                        TextMode="Number" 
                        Text="0">
                    </asp:TextBox>
                </div>

                <div class="col-md-3">
                    <label class="form-label">مسافة البداية (كم)</label>
                    <asp:TextBox ID="txtWriteStartKM" runat="server" 
                        CssClass="form-control" 
                        TextMode="Number" 
                        step="0.1" 
                        Text="0"
                        AutoPostBack="true"
                        OnTextChanged="txtWriteStartKM_TextChanged">
                    </asp:TextBox>
                </div>

                <div class="col-md-3">
                    <label class="form-label">مسافة النهاية (كم)</label>
                    <asp:TextBox ID="txtWriteEndKM" runat="server" 
                        CssClass="form-control" 
                        TextMode="Number" 
                        step="0.1" 
                        Text="0"
                        AutoPostBack="true"
                        OnTextChanged="txtWriteEndKM_TextChanged">
                    </asp:TextBox>
                </div>

                <div class="col-md-3">
                    <label class="form-label">نتيجة المسافة (كم)</label>
                    <asp:TextBox ID="txtResultKM" runat="server" 
                        CssClass="form-control readonly-field" 
                        TextMode="Number" 
                        step="0.1" 
                        Text="0"
                        ReadOnly="true">
                    </asp:TextBox>
                </div>
            </div>

            <!-- الصف الخامس: المركبات والرقم القومي -->
            <div class="row">
                <div class="col-md-4">
                    <label class="form-label">عدد المركبات</label>
                    <asp:TextBox ID="txtCarCount" runat="server" 
                        CssClass="form-control" 
                        TextMode="Number" 
                        Text="0">
                    </asp:TextBox>
                </div>

                <div class="col-md-4">
                    <label class="form-label required">الرقم القومي</label>
                    <asp:TextBox ID="txtNationalID" runat="server" 
                        CssClass="form-control" 
                        placeholder="أدخل الرقم القومي (14 رقم)"
                        MaxLength="14">
                    </asp:TextBox>
                    <div class="info-text">يجب أن يتكون من 14 رقم</div>
                </div>

                <div class="col-md-4">
                    <label class="form-label">رقم الموظف (اختياري)</label>
                    <asp:TextBox ID="txtEmployeeID" runat="server" 
                        CssClass="form-control" 
                        TextMode="Number"
                        placeholder="أدخل رقم الموظف إن وجد">
                    </asp:TextBox>
                </div>
            </div>

            <!-- صف الخطوط -->
            <div class="row mt-3">
                <div class="col-md-12">
                    <label class="form-label">الخطوط المتاحة</label>
                    <div class="routes-container">
                        <asp:ListBox ID="lstRoutes" runat="server" 
                            CssClass="form-control" 
                            SelectionMode="Multiple"
                            Rows="6"
                            Style="height: auto; min-height: 150px;">
                        </asp:ListBox>
                    </div>
                    <div class="info-text">
                        <i class="fas fa-info-circle"></i> يمكنك اختيار أكثر من خط بالضغط على Ctrl + زر الفأرة
                    </div>
                </div>
            </div>

            <!-- الحالة -->
            <div class="row mt-3">
                <div class="col-md-12">
                    <div class="checkbox-container">
                        <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
                        <label>نشط</label>
                    </div>
                </div>
            </div>

            <!-- الأزرار -->
            <div class="row mt-4">
                <div class="col-md-6">
                    <asp:Button ID="btnSave" runat="server" 
                        Text="حفظ البيانات" 
                        CssClass="btn btn-save w-100"
                        OnClick="btnSave_Click" />
                </div>
                <div class="col-md-6">
                    <asp:Button ID="btnClear" runat="server" 
                        Text="مسح الحقول" 
                        CssClass="btn btn-clear w-100"
                        OnClick="btnClear_Click" />
                </div>
            </div>
        </div>

        <!-- ربط بصفحة الاستعلام -->
        <div class="text-center mt-3">
            <a href="DriverInquiry.aspx" class="btn btn-link">
                <i class="fas fa-arrow-left"></i> الذهاب إلى صفحة الاستعلام
            </a>
       
        </div>

    </div>

    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">

    <!-- JavaScript للتحقق الإضافي -->
    <script type="text/javascript">
        function validateForm() {
            // التحقق من الرقم القومي
            var nationalID = document.getElementById('<%= txtNationalID.ClientID %>').value;
            if (nationalID.length !== 14 || isNaN(nationalID)) {
                alert('الرجاء إدخال رقم قومي صحيح مكون من 14 رقم');
                return false;
            }
            return true;
        }
    </script>

</asp:Content>