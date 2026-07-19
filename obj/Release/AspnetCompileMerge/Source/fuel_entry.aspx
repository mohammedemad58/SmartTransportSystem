<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="fuel_entry.aspx.cs" Inherits="car_WebApplication1.fuel_entry" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <title>إدارة الوقود والكروت</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f8f9fa;
        }
        .entry-container {
            background-color: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
            margin-top: 30px;
        }
        .page-title {
            color: #a50d25;
            font-weight: bold;
            margin-bottom: 25px;
            padding-bottom: 10px;
            border-bottom: 2px solid #c8102e;
        }
        .btn-save {
            background-color: #a50d25;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 38px;
        }
        .btn-save:hover {
            background-color: #a50d25;
            border-color: #a50d25;
        }
        .btn-reset {
            background-color: #6c757d;
            border-color: #6c757d;
            color: white;
            font-weight: bold;
            height: 38px;
        }
        .btn-reset:hover {
            background-color: #5a6268;
            border-color: #545b62;
        }
        .btn-card {
            background-color: #17a2b8;
            border-color: #17a2b8;
            color: white;
            font-weight: bold;
            height: 38px;
        }
        .btn-card:hover {
            background-color: #138496;
            border-color: #117a8b;
        }
        .btn-tab {
            background-color: transparent;
            border: none;
            color: #495057;
            font-weight: bold;
            padding: 10px 20px;
            margin: 0 2px;
            border-radius: 0;
        }
        .btn-tab.active {
            color: #a50d25;
            border-bottom: 3px solid #c8102e;
            background-color: transparent;
        }
        .btn-tab:hover {
            color: #a50d25;
        }
        .form-section {
            background-color: #f8f9fa;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
            border-right: 4px solid #c8102e;
        }
        .section-title {
            color: #a50d25;
            font-weight: bold;
            margin-bottom: 15px;
            font-size: 1.1rem;
        }
        .required-field label::after {
            content: " *";
            color: #c8102e;
            font-weight: bold;
        }
        .card-item {
            background-color: white;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 10px;
            border: 1px solid #dee2e6;
            border-right: 3px solid #c8102e;
        }
        .card-number-display {
            font-size: 1.2rem;
            font-weight: bold;
            color: #c8102e;
            direction: ltr;
            text-align: left;
            font-family: monospace;
        }
        .table-header {
            background-color: #a50d25;
            color: white;
        }
        .summary-item {
            background-color: #f8f9fa;
            border-radius: 10px;
            padding: 15px;
            margin: 10px 0;
            border-right: 4px solid #c8102e;
            text-align: center;
        }
        .summary-label {
            font-size: 14px;
            color: #6c757d;
            margin-bottom: 5px;
        }
        .summary-value {
            font-size: 20px;
            font-weight: bold;
            color: #343a40;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4" dir="rtl">
        <div class="entry-container">
            
            <h3 class="page-title text-center">
                <i class="fas fa-gas-pump"></i> إدارة الوقود والكروت
            </h3>

            <asp:Panel ID="pnlMessage" runat="server" CssClass="alert" Visible="false" role="alert">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Tab Buttons using ASP.NET Buttons -->
            <div class="text-center mb-4" style="border-bottom: 2px solid #dee2e6;">
               <asp:Button ID="btnTabFuel" runat="server" Text="تعبئة وقود" 
    CssClass="btn-tab active" OnClick="btnTabFuel_Click" CausesValidation="false" />
<asp:Button ID="btnTabCards" runat="server" Text="إدارة الكروت" 
    CssClass="btn-tab" OnClick="btnTabCards_Click" CausesValidation="false" />
<asp:Button ID="btnTabTransactions" runat="server" Text="معاملات الكروت" 
    CssClass="btn-tab" OnClick="btnTabTransactions_Click" CausesValidation="false" />
                    
            </div>

            <!-- MultiView for tab content -->
            <asp:MultiView ID="MainMultiView" runat="server" ActiveViewIndex="0">
                
                <!-- Fuel Entry View -->
                <asp:View ID="viewFuel" runat="server">
                    <div class="form-section">
                        <div class="section-title">
                            <i class="fas fa-car"></i> بيانات السيارة
                        </div>
                        <div class="row g-3">
                            <div class="col-md-6 required-field">
                                <label class="form-label fw-bold">اختر السيارة</label>
                                <asp:DropDownList ID="ddlVehicle" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- اختر السيارة --" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvVehicle" runat="server" 
                                    ControlToValidate="ddlVehicle" InitialValue="0"
                                    ErrorMessage="الرجاء اختيار السيارة" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label fw-bold">عداد السيارة</label>
                                <asp:TextBox ID="txtOdometer" runat="server" CssClass="form-control" 
                                    TextMode="Number" placeholder="قراءة العداد"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label fw-bold">المسافة المقطوعة</label>
                                <asp:TextBox ID="txtDistance" runat="server" CssClass="form-control" 
                                    TextMode="Number" placeholder="كم"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <div class="section-title">
                            <i class="fas fa-oil-can"></i> بيانات الوقود
                        </div>
                        <div class="row g-3">
                            <div class="col-md-3 required-field">
                                <label class="form-label fw-bold">تاريخ التعبئة</label>
                                <asp:TextBox ID="txtRegDate" runat="server" TextMode="Date" 
                                    CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvRegDate" runat="server" 
                                    ControlToValidate="txtRegDate"
                                    ErrorMessage="الرجاء إدخال التاريخ" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-3 required-field">
                                <label class="form-label fw-bold">نوع الوقود</label>
                                <asp:DropDownList ID="ddlFuelType" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- اختر النوع --" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvFuelType" runat="server" 
                                    ControlToValidate="ddlFuelType" InitialValue="0"
                                    ErrorMessage="الرجاء اختيار نوع الوقود" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-3 required-field">
                                <label class="form-label fw-bold">عدد اللترات</label>
                                <asp:TextBox ID="txtLiters" runat="server" CssClass="form-control" 
                                    TextMode="Number" step="0.01" placeholder="0.00" onkeyup="calculateTotal()" onchange="calculateTotal()"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLiters" runat="server" 
                                    ControlToValidate="txtLiters"
                                    ErrorMessage="الرجاء إدخال عدد اللترات" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-3 required-field">
                                <label class="form-label fw-bold">سعر اللتر</label>
                                <asp:TextBox ID="txtLiterPrice" runat="server" CssClass="form-control" 
                                    TextMode="Number" step="0.01" placeholder="0.00" onkeyup="calculateTotal()" onchange="calculateTotal()"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLiterPrice" runat="server" 
                                    ControlToValidate="txtLiterPrice"
                                    ErrorMessage="الرجاء إدخال سعر اللتر" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="row g-3 mt-2">
                            <div class="col-md-4">
                                <label class="form-label fw-bold">إجمالي التكلفة</label>
                                <asp:TextBox ID="txtTotalCost" runat="server" CssClass="form-control" 
                                    ReadOnly="true" placeholder="0.00"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label fw-bold">المحطة</label>
                                <asp:TextBox ID="txtStation" runat="server" CssClass="form-control" 
                                    placeholder="اسم المحطة"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label fw-bold">رقم الكارت</label>
                                <asp:DropDownList ID="ddlCardNumber" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- بدون كارت --" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-12">
                                <label class="form-label fw-bold">ملاحظات</label>
                                <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" 
                                    TextMode="MultiLine" Rows="2" placeholder="أي ملاحظات إضافية..."></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6 d-flex gap-2">
                            <asp:Button ID="btnSaveFuel" runat="server" Text="حفظ بيانات الوقود" 
                                CssClass="btn btn-save" OnClick="btnSaveFuel_Click" />
                            <asp:Button ID="btnResetFuel" runat="server" Text="إعادة تعيين" 
                                CssClass="btn btn-reset" OnClick="btnResetFuel_Click" CausesValidation="false" />
                        </div>
                    </div>
                </asp:View>

                <!-- Cards Management View -->
                <asp:View ID="viewCards" runat="server">
                    <div class="form-section">
                        <div class="section-title">
                            <i class="fas fa-plus-circle"></i> إضافة كارت جديد
                        </div>
                        <div class="row g-3">
                            <div class="col-md-6 required-field">
                                <label class="form-label fw-bold">رقم الكارت</label>
                                <asp:TextBox ID="txtNewCardNumber" runat="server" CssClass="form-control" 
                                    placeholder="أدخل رقم الكارت" dir="ltr"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvNewCard" runat="server" 
                                    ControlToValidate="txtNewCardNumber"
                                    ErrorMessage="الرجاء إدخال رقم الكارت" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4 d-flex align-items-end">
                                <asp:Button ID="btnAddCard" runat="server" Text="إضافة كارت" 
                                    CssClass="btn btn-card" OnClick="btnAddCard_Click" />
                            </div>
                        </div>
                    </div>

                    <div class="section-title mt-4">
                        <i class="fas fa-list"></i> قائمة الكروت الموجودة
                    </div>
                    
                    <div class="row">
                        <asp:Repeater ID="rptCards" runat="server" OnItemCommand="rptCards_ItemCommand">
                            <ItemTemplate>
                                <div class="col-md-6">
                                    <div class="card-item">
                                        <div class="d-flex justify-content-between align-items-center">
                                            <div>
                                                <span class="card-number-display"><%# Eval("card_number") %></span>
                                                <small class="text-muted d-block">كارت رقم <%# Container.ItemIndex + 1 %></small>
                                            </div>
                                            <div>
                                                <asp:LinkButton ID="btnDeleteCard" runat="server" 
                                                    CommandName="DeleteCard" 
                                                    CommandArgument='<%# Eval("card_id") %>'
                                                    CssClass="btn btn-sm btn-outline-danger"
                                                    CausesValidation="false"
                                                    OnClientClick="return confirm('هل أنت متأكد من حذف هذا الكارت؟');">
                                                    <i class="fas fa-trash"></i> حذف
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    
                    <asp:Panel ID="pnlNoCards" runat="server" CssClass="text-center text-muted p-5" Visible="false">
                        <i class="fas fa-credit-card fa-3x mb-3"></i>
                        <h5>لا توجد كروت مضافة</h5>
                    </asp:Panel>
                </asp:View>

                <!-- Transactions View -->
                <asp:View ID="viewTransactions" runat="server">
                    <div class="form-section">
                        <div class="section-title">
                            <i class="fas fa-exchange-alt"></i> إضافة معاملة كارت
                        </div>
                        <div class="row g-3">
                            <div class="col-md-3 required-field">
                                <label class="form-label fw-bold">اختر الكارت</label>
                                <asp:DropDownList ID="ddlTransactionCard" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- اختر الكارت --" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTransactionCard" runat="server" 
                                    ControlToValidate="ddlTransactionCard" InitialValue="0"
                                    ErrorMessage="الرجاء اختيار الكارت" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label fw-bold">اختر السيارة</label>
                                <asp:DropDownList ID="ddlTransactionVehicle" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- اختر السيارة --" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2 required-field">
                                <label class="form-label fw-bold">نوع المعاملة</label>
                                <asp:DropDownList ID="ddlTransactionType" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="إيداع" Value="إيداع"></asp:ListItem>
                                    <asp:ListItem Text="سحب" Value="سحب"></asp:ListItem>
                                    <asp:ListItem Text="تعبئة" Value="تعبئة"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2 required-field">
                                <label class="form-label fw-bold">المبلغ</label>
                                <asp:TextBox ID="txtTransactionAmount" runat="server" CssClass="form-control" 
                                    TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvTransactionAmount" runat="server" 
                                    ControlToValidate="txtTransactionAmount"
                                    ErrorMessage="الرجاء إدخال المبلغ" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-2 required-field">
                                <label class="form-label fw-bold">التاريخ</label>
                                <asp:TextBox ID="txtTransactionDate" runat="server" TextMode="Date" 
                                    CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvTransactionDate" runat="server" 
                                    ControlToValidate="txtTransactionDate"
                                    ErrorMessage="الرجاء إدخال التاريخ" 
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-md-12">
                                <asp:Button ID="btnAddTransaction" runat="server" Text="إضافة معاملة" 
                                    CssClass="btn btn-save" OnClick="btnAddTransaction_Click" />
                            </div>
                        </div>
                    </div>

                    <div class="section-title mt-4">
                        <i class="fas fa-history"></i> آخر المعاملات
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="gvTransactions" runat="server"
                            CssClass="table table-bordered table-striped table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد معاملات"
                            EmptyDataRowStyle-CssClass="text-center text-muted p-5"
                            HeaderStyle-CssClass="table-header"
                            DataKeyNames="transaction_id"
                            OnRowCommand="gvTransactions_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="transaction_date" HeaderText="التاريخ" />
                                <asp:BoundField DataField="card_number" HeaderText="رقم الكارت" />
                                <asp:BoundField DataField="vehicle_number" HeaderText="السيارة" />
                                <asp:BoundField DataField="transaction_type" HeaderText="نوع المعاملة" />
                                <asp:BoundField DataField="amount" HeaderText="المبلغ" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="إجراءات">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDeleteTransaction" runat="server" 
                                            CommandName="DeleteTransaction" 
                                            CommandArgument='<%# Eval("transaction_id") %>'
                                            CssClass="btn btn-sm btn-outline-danger"
                                            CausesValidation="false"
                                            OnClientClick="return confirm('هل أنت متأكد من حذف هذه المعاملة؟');">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <!-- Transaction Summary -->
                    <div class="row mt-4">
                        <div class="col-md-4">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-arrow-up"></i> إجمالي الإيداعات
                                </div>
                                <div class="summary-value text-success">
                                    <asp:Label ID="lblTotalDeposits" runat="server" Text="0.00"></asp:Label> ج
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-arrow-down"></i> إجمالي المسحوبات
                                </div>
                                <div class="summary-value text-danger">
                                    <asp:Label ID="lblTotalWithdrawals" runat="server" Text="0.00"></asp:Label> ج
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-balance-scale"></i> الرصيد الحالي
                                </div>
                                <div class="summary-value text-primary">
                                    <asp:Label ID="lblCurrentBalance" runat="server" Text="0.00"></asp:Label> ج
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:View>
            </asp:MultiView>

            <hr class="my-4" />

            <div class="text-start">
                <asp:HyperLink ID="hlBackToReport" runat="server" 
                    NavigateUrl="~/fuel_report.aspx" CssClass="btn btn-outline-secondary">
                    <i class="fas fa-chart-bar"></i> الذهاب للتقرير
                </asp:HyperLink>
            </div>

        </div>
    </div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    
    <script type="text/javascript">
        function calculateTotal() {
            var liters = document.getElementById('<%= txtLiters.ClientID %>').value;
            var price = document.getElementById('<%= txtLiterPrice.ClientID %>').value;
            
            if (liters && price && !isNaN(liters) && !isNaN(price)) {
                var total = parseFloat(liters) * parseFloat(price);
                document.getElementById('<%= txtTotalCost.ClientID %>').value = total.toFixed(2);
            } else {
                document.getElementById('<%= txtTotalCost.ClientID %>').value = '';
            }
        }
    </script>
</asp:Content>