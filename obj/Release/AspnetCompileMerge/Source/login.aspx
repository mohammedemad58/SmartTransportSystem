<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="login.aspx.cs"
    Inherits="cars_WebApplication1.login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" dir="rtl" lang="ar">
<head runat="server">
    <title>تسجيل الدخول</title>
    
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            background-color: #f8f9fa;
            direction: rtl;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .login-wrapper {
            width: 100%;
            max-width: 650px;  /* Increased from 450px to 650px */
            margin: 30px;
        }

        .login-card {
            background: white;
            border-radius: 15px;  /* Increased from 10px to 15px */
            box-shadow: 0 0 30px rgba(0,0,0,0.15);  /* Increased shadow */
            padding: 50px;  /* Increased from 30px to 50px */
        }

        .login-header {
            text-align: center;
            margin-bottom: 45px;  /* Increased from 30px to 45px */
            padding-bottom: 25px;  /* Increased from 20px to 25px */
            border-bottom: 3px solid #c8102e;  /* Thicker border */
        }

        .login-header h3 {
            color: #333;
            font-weight: bold;
            margin: 0;
            font-size: 32px;  /* Added larger font size */
        }

        .form-group {
            margin-bottom: 30px;  /* Increased from 20px to 30px */
        }

        .form-label {
            font-weight: bold;
            color: #333;
            margin-bottom: 12px;  /* Increased from 8px to 12px */
            display: block;
            font-size: 16px;  /* Added larger font size */
        }

        .form-control {
            height: 55px;  /* Increased from 45px to 55px */
            border: 2px solid #ddd;  /* Thicker border */
            border-radius: 8px;  /* Increased from 6px to 8px */
            padding: 0 15px;  /* Increased padding */
            font-size: 16px;  /* Larger font size */
            width: 100%;
        }

        .form-control:focus {
            border-color: #c8102e;
            outline: none;
            box-shadow: 0 0 0 3px rgba(200,16,46,0.1);  /* Larger focus effect */
        }

        .btn-login {
            background-color: #c8102e;
            color: white;
            border: none;
            border-radius: 8px;  /* Increased from 6px to 8px */
            height: 55px;  /* Increased from 45px to 55px */
            font-weight: bold;
            font-size: 18px;  /* Larger font size */
            width: 100%;
            cursor: pointer;
            transition: all 0.3s;
            margin-top: 10px;  /* Added margin top */
        }

        .btn-login:hover {
            background-color: #a50d25;
        }

        .alert-box {
            background-color: #fee2e2;
            border: 2px solid #fecaca;  /* Thicker border */
            color: #dc2626;
            padding: 15px;  /* Increased from 10px to 15px */
            border-radius: 8px;  /* Increased from 6px to 8px */
            margin-bottom: 30px;  /* Increased from 20px to 30px */
            text-align: center;
            font-size: 15px;  /* Larger font size */
        }

        .login-footer {
            text-align: center;
            margin-top: 35px;  /* Increased from 25px to 35px */
            padding-top: 25px;  /* Increased from 20px to 25px */
            border-top: 2px solid #eee;  /* Thicker border */
            font-size: 14px;  /* Larger font size */
            color: #666;
        }

        /* Responsive adjustments */
        @media (max-width: 768px) {
            .login-wrapper {
                max-width: 90%;
                margin: 20px;
            }
            
            .login-card {
                padding: 30px;
            }
            
            .login-header h3 {
                font-size: 28px;
            }
            
            .form-control, .btn-login {
                height: 50px;
            }
        }

        @media (max-width: 576px) {
            .login-card {
                padding: 25px;
            }
            
            .login-header h3 {
                font-size: 24px;
            }
            
            .login-header {
                margin-bottom: 30px;
            }
            
            .form-group {
                margin-bottom: 20px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-wrapper">
            <div class="login-card">
                
                <div class="login-header">
                    <h3>تسجيل الدخول</h3>
                </div>

                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <div class="alert-box">
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </div>
                </asp:Panel>

                <div class="form-group">
                    <label class="form-label">اسم المستخدم</label>
                    <asp:TextBox ID="inputEmail" runat="server" CssClass="form-control" placeholder="أدخل اسم المستخدم"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                        ControlToValidate="inputEmail"
                        ErrorMessage="الرجاء إدخال اسم المستخدم" 
                        CssClass="text-danger small mt-2"
                        Display="Dynamic"
                        ValidationGroup="Login">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="form-group">
                    <label class="form-label">كلمة المرور</label>
                    <asp:TextBox ID="inputPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="أدخل كلمة المرور"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
                        ControlToValidate="inputPassword"
                        ErrorMessage="الرجاء إدخال كلمة المرور" 
                        CssClass="text-danger small mt-2"
                        Display="Dynamic"
                        ValidationGroup="Login">
                    </asp:RequiredFieldValidator>
                </div>

                <asp:Button ID="btnSignIn" runat="server" CssClass="btn-login" Text="تسجيل الدخول" OnClick="btnSignIn_Click" ValidationGroup="Login" />

                <div class="login-footer">
                    <span>نظام إدارة المركبات © 2026</span>
                </div>

            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>