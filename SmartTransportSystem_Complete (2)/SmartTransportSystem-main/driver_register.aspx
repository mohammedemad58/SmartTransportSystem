<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="driver_register.aspx.cs"
    Inherits="car_WebApplication1.driver_register" %>

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

        .form-control, .enhanced-dropdown {
            text-align: right;
            background: #fff;
            border: 2px solid #dee2e6;
            border-radius: 8px;
            transition: all 0.3s ease;
            height: 45px;
            width: 100%;
            padding: 0 10px;
        }

        .form-control:focus, .enhanced-dropdown:focus {
            border-color: #c8102e;
            box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1);
            outline: none;
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

        .btn-warning {
            background: #ffc107;
            color: #2c3e50;
            border: none;
            padding: 5px 12px;
            border-radius: 4px;
        }

        .btn-warning:hover {
            background: #e0a800;
        }

        .form-group {
            margin-bottom: 18px;
        }
    </style>

    <div class="container-fluid" style="padding: 25px;">

        <h2 class="page-title">
            <i class="fas fa-user-plus"></i>
            تسجيل حسابات السائقين
        </h2>

        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert" style="border-radius:10px; margin-bottom:20px; font-weight:bold;">
            <asp:Literal ID="litMessage" runat="server"></asp:Literal>
        </asp:Panel>

        <!-- Registration form -->
        <div class="search-box">
            <div class="section-header">
                <i class="fas fa-id-card"></i>
                بيانات الحساب الجديد
            </div>

            <div class="row">
                <div class="col-md-4 form-group">
                    <label class="form-label">السائق (من ملف الموظفين - وظيفة "سائق")</label>
                    <asp:DropDownList ID="ddlDriverEmployee" runat="server" CssClass="enhanced-dropdown">
                    </asp:DropDownList>
                </div>

                <div class="col-md-4 form-group">
                    <label class="form-label">اسم المستخدم</label>
                    <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="اسم الدخول"></asp:TextBox>
                </div>

                <div class="col-md-4 form-group">
                    <label class="form-label">كلمة المرور</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="كلمة المرور"></asp:TextBox>
                </div>

                <div class="col-md-4 form-group">
                    <label class="form-label">تأكيد كلمة المرور</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="أعد كتابة كلمة المرور"></asp:TextBox>
                </div>
            </div>

            <div class="row mt-3">
                <div class="col-md-3">
                    <asp:Button ID="btnSave" runat="server" Text="حفظ الحساب" CssClass="btn btn-save w-100" OnClick="btnSave_Click" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnClear" runat="server" Text="مسح الحقول" CssClass="btn btn-clear w-100" OnClick="btnClear_Click" CausesValidation="false" />
                </div>
            </div>
        </div>

        <!-- Existing driver accounts -->
        <div class="result-box">
            <div class="section-header">
                <i class="fas fa-list"></i>
                حسابات السائقين الحالية
            </div>

            <div class="table-responsive">
                <asp:GridView ID="gvDriverLogins" runat="server" AutoGenerateColumns="false"
                    CssClass="table table-bordered" GridLines="None" Width="100%"
                    EmptyDataText="لا توجد حسابات سائقين مسجلة بعد"
                    OnRowCommand="gvDriverLogins_RowCommand" DataKeyNames="user_id">
                    <HeaderStyle CssClass="table-header" />
                    <RowStyle CssClass="table-row" />
                    <AlternatingRowStyle CssClass="table-alt-row" />
                    <Columns>
                        <asp:BoundField DataField="user_id" HeaderText="#" />
                        <asp:BoundField DataField="user_name" HeaderText="اسم المستخدم" />
                        <asp:BoundField DataField="employee_name" HeaderText="اسم السائق" />
                        <asp:BoundField DataField="created_at" HeaderText="تاريخ الإنشاء" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:TemplateField HeaderText="إجراءات">
                            <ItemTemplate>
                                <asp:Button ID="btnResetPassword" runat="server" Text="إعادة تعيين كلمة المرور"
                                    CssClass="btn btn-warning btn-sm"
                                    CommandName="ResetPassword"
                                    CommandArgument='<%# Eval("user_id") %>'
                                    OnClientClick="return confirm('سيتم تعيين كلمة مرور جديدة لهذا الحساب. متابعة؟');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </div>

</asp:Content>
