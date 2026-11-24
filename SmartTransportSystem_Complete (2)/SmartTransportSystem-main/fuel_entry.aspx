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
        .readonly-field {
            background-color: #e9ecef !important;
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
        .transaction-number-col {
            font-family: monospace;
            font-weight: bold;
            direction: ltr;
        }
        .card-balance-hint {
            color: #0d6efd;
            font-weight: bold;
        }
        .transactions-scroll {
            max-height: 420px;
            overflow-y: auto;
        }
        .transactions-scroll table thead th {
            position: sticky;
            top: 0;
            z-index: 1;
        }
        .card-pager table {
            margin: 15px auto 0 auto;
            border-collapse: separate;
            border-spacing: 4px;
        }
        .card-pager td {
            padding: 0 !important;
        }
        .card-pager a, .card-pager span {
            display: inline-block;
            min-width: 36px;
            padding: 6px 10px;
            border-radius: 8px;
            border: 1px solid #dee2e6;
            font-weight: bold;
            text-decoration: none;
            text-align: center;
        }
        .card-pager a {
            color: #a50d25;
            background-color: #fff;
        }
        .card-pager a:hover {
            background-color: #f8d7da;
            border-color: #c8102e;
        }
        .card-pager span {
            color: #fff;
            background-color: #a50d25;
            border-color: #a50d25;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4" dir="rtl">
        <div class="entry-container">

            <h3 class="page-title text-center">
                <i class="fas fa-gas-pump"></i> إدارة الوقود
            </h3>

            <asp:Panel ID="pnlMessage" runat="server" CssClass="alert" Visible="false" role="alert">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Tab Buttons -->
            <div class="text-center mb-4" style="border-bottom: 2px solid #dee2e6;">
                <asp:Button ID="btnTabFuel" runat="server" Text="تعبئة وقود"
                    CssClass="btn-tab active" OnClick="btnTabFuel_Click" CausesValidation="false" />
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
                            <div class="col-md-4 required-field">
                                <label class="form-label fw-bold">اختر السيارة</label>
                                <asp:DropDownList ID="ddlVehicle" runat="server" CssClass="form-select"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlVehicle_SelectedIndexChanged">
                                    <asp:ListItem Text="-- اختر السيارة --" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvVehicle" runat="server"
                                    ControlToValidate="ddlVehicle" InitialValue="0"
                                    ErrorMessage="الرجاء اختيار السيارة"
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </div>

                            <div class="col-md-4">
                                <label class="form-label fw-bold">مزودة بغاز طبيعي</label>
                                <asp:TextBox ID="txtHasNaturalGas" runat="server" CssClass="form-control readonly-field"
                                    ReadOnly="true" placeholder="سيتم عرض الحالة تلقائياً"></asp:TextBox>
                            </div>

                            <div class="col-md-4 required-field">
                                <label class="form-label fw-bold">العداد الحالي (كم)</label>
                                <asp:TextBox ID="txtCurrentOdometer" runat="server" CssClass="form-control"
                                    TextMode="Number" step="0.1" min="0" placeholder="أدخل قراءة العداد الحالية"
                                    OnTextChanged="txtCurrentOdometer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvOdometer" runat="server"
                                    ControlToValidate="txtCurrentOdometer"
                                    ErrorMessage="الرجاء إدخال قراءة العداد الحالية"
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                                <small class="text-muted">أدخل قراءة العداد الحالية للسيارة</small>
                            </div>

                            <div class="col-md-4">
                                <label class="form-label fw-bold">المسافة المقطوعة منذ آخر تعبئة (كم)</label>
                                <asp:TextBox ID="txtDistance" runat="server" CssClass="form-control readonly-field"
                                    ReadOnly="true" placeholder="سيتم حسابها تلقائياً"></asp:TextBox>
                                <small class="text-muted">تحسب تلقائياً من فرق العداد</small>
                            </div>
                        </div>

                        <div class="row g-3 mt-2">
                            <div class="col-md-6">
                                <div class="alert alert-info">
                                    <i class="fas fa-info-circle"></i>
                                    <strong>آخر تعبئة:</strong>
                                    <asp:Label ID="lblLastFuelInfo" runat="server" Text="لا توجد تعبئة سابقة"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="alert alert-success" id="consumptionAlert" runat="server" visible="false">
                                    <i class="fas fa-chart-line"></i>
                                    <strong>معدل الاستهلاك السابق:</strong>
                                    <asp:Label ID="lblPreviousConsumption" runat="server" Text="0.00"></asp:Label>
                                    <asp:Label ID="lblConsumptionUnit" runat="server" Text="كم/لتر"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <div class="section-title">
                            <i class="fas fa-oil-can"></i> بيانات التعبئة
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

                            <!-- Fuel (Liquid) Fields - Always Visible -->
                            <div id="divLiters" runat="server" class="col-md-3 required-field">
                                <label class="form-label fw-bold">عدد اللترات</label>
                                <asp:TextBox ID="txtLiters" runat="server" CssClass="form-control"
                                    TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                                <small class="text-muted">لتر</small>
                            </div>

                            <!-- Natural Gas Fields - Visible only for gas vehicles -->
                            <div class="row g-3 mt-2" id="naturalGasFields" runat="server">
                                <div class="col-md-12">
                                    <hr />
                                    <h6 class="text-primary"><i class="fas fa-fire"></i> بيانات الغاز الطبيعي</h6>
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label fw-bold">عدد الأمتار المكعبة</label>
                                    <asp:TextBox ID="txtCubicMeters" runat="server" CssClass="form-control"
                                        TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                                    <small class="text-muted">متر مكعب (م³)</small>
                                </div>
                            </div>

                            <div class="row g-3 mt-2">
                                <div class="col-md-4">
                                    <label class="form-label fw-bold">المحطة</label>
                                    <asp:TextBox ID="txtStation" runat="server" CssClass="form-control"
                                        placeholder="اسم المحطة"></asp:TextBox>
                                </div>

                                <!-- Card: MANDATORY -->
                                <div class="col-md-4 required-field">
                                    <label class="form-label fw-bold">رقم الكارت</label>
                                    <asp:DropDownList ID="ddlCardNumber" runat="server" CssClass="form-select"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlCardNumber_SelectedIndexChanged">
                                        <asp:ListItem Text="-- اختر الكارت --" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvCardNumber" runat="server"
                                        ControlToValidate="ddlCardNumber" InitialValue="0"
                                        ErrorMessage="الرجاء اختيار رقم الكارت"
                                        CssClass="text-danger small" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                    <small class="card-balance-hint">
                                        <asp:Label ID="lblCardBalance" runat="server" Text=""></asp:Label>
                                    </small>
                                </div>

                                <div class="col-md-4">
                                    <label class="form-label fw-bold">ملاحظات</label>
                                    <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control"
                                        TextMode="MultiLine" Rows="1" placeholder="أي ملاحظات إضافية..."></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row mt-3">
                        <div class="col-md-6 d-flex gap-2">
                            <asp:Button ID="btnSaveFuel" runat="server" Text="حفظ بيانات التعبئة"
                                CssClass="btn btn-save" OnClick="btnSaveFuel_Click" />
                            <asp:Button ID="btnResetFuel" runat="server" Text="إعادة تعيين"
                                CssClass="btn btn-reset" OnClick="btnResetFuel_Click" CausesValidation="false" />
                        </div>
                    </div>
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
                                <asp:DropDownList ID="ddlTransactionCard" runat="server" CssClass="form-select"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlTransactionCard_SelectedIndexChanged">
                                    <asp:ListItem Text="-- اختر الكارت --" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTransactionCard" runat="server"
                                    ControlToValidate="ddlTransactionCard" InitialValue="0"
                                    ErrorMessage="الرجاء اختيار الكارت"
                                    CssClass="text-danger small" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                                <small class="card-balance-hint">
                                    <asp:Label ID="lblTransactionCardBalance" runat="server" Text=""></asp:Label>
                                </small>
                            </div>
                            <div class="col-md-2 required-field">
                                <label class="form-label fw-bold">نوع المعاملة</label>
                                <asp:DropDownList ID="ddlTransactionType" runat="server" CssClass="form-select"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlTransactionType_SelectedIndexChanged">
                                    <asp:ListItem Text="شحن" Value="شحن"></asp:ListItem>
                                    <asp:ListItem Text="تحويل" Value="تحويل"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2 required-field" id="divToCard" runat="server" visible="false">
                                <label class="form-label fw-bold">تحويل إلى كارت</label>
                                <asp:DropDownList ID="ddlTransferToCard" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- اختر الكارت --" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2 required-field">
                                <label class="form-label fw-bold">الوحدة</label>
                                <asp:DropDownList ID="ddlTransactionUnit" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="لتر" Value="liter"></asp:ListItem>
                                    <asp:ListItem Text="متر مكعب" Value="cubic"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2 required-field">
                                <label class="form-label fw-bold">الكمية</label>
                                <asp:TextBox ID="txtTransactionAmount" runat="server" CssClass="form-control"
                                    TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvTransactionAmount" runat="server"
                                    ControlToValidate="txtTransactionAmount"
                                    ErrorMessage="الرجاء إدخال الكمية"
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
                                <asp:Button ID="btnAddTransaction" runat="server" Text="تنفيذ المعاملة"
                                    CssClass="btn btn-save" OnClick="btnAddTransaction_Click" />
                            </div>
                        </div>
                    </div>

                    <div class="section-title mt-4">
                        <i class="fas fa-history"></i> آخر المعاملات
                    </div>

                    <div class="table-responsive transactions-scroll">
                        <asp:GridView ID="gvTransactions" runat="server"
                            CssClass="table table-bordered table-striped table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد معاملات"
                            EmptyDataRowStyle-CssClass="text-center text-muted p-5"
                            HeaderStyle-CssClass="table-header"
                            DataKeyNames="transaction_id"
                            OnRowCommand="gvTransactions_RowCommand"
                            AllowPaging="true"
                            PageSize="10"
                            OnPageIndexChanging="gvTransactions_PageIndexChanging"
                            PagerSettings-Mode="NumericFirstLast"
                            PagerSettings-Position="Bottom"
                            PagerSettings-PageButtonCount="5"
                            PagerStyle-CssClass="card-pager"
                            PagerStyle-HorizontalAlign="Center">
                            <Columns>
                                <asp:BoundField DataField="transaction_number" HeaderText="رقم المعاملة" ItemStyle-CssClass="transaction-number-col" />
                                <asp:BoundField DataField="transaction_date" HeaderText="التاريخ" />
                                <asp:BoundField DataField="card_number" HeaderText="الكارت" />
                                <asp:BoundField DataField="vehicle_number" HeaderText="رقم السيارة" />
                                <asp:BoundField DataField="transaction_type" HeaderText="نوع المعاملة" />
                                <asp:BoundField DataField="quantity" HeaderText="الكمية" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="unit_display" HeaderText="الوحدة" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="to_card_number" HeaderText="إلى كارت" />
                                <asp:TemplateField HeaderText="إجراءات">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDeleteTransaction" runat="server"
                                            CommandName="DeleteTransaction"
                                            CommandArgument='<%# Eval("transaction_id") %>'
                                            CssClass="btn btn-sm btn-outline-danger"
                                            CausesValidation="false"
                                            OnClientClick="return confirm('هل أنت متأكد من حذف هذه المعاملة؟ سيتم إرجاع تأثيرها على الرصيد.');">
                                            <i class="fas fa-trash"></i> حذف
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
                                    <i class="fas fa-plus-circle"></i> إجمالي الشحن (لتر)
                                </div>
                                <div class="summary-value text-success">
                                    <asp:Label ID="lblTotalChargeLiters" runat="server" Text="0.00"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-plus-circle"></i> إجمالي الشحن (م³)
                                </div>
                                <div class="summary-value text-info">
                                    <asp:Label ID="lblTotalChargeCubic" runat="server" Text="0.00"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-minus-circle"></i> إجمالي الاستهلاك (خصم تلقائي)
                                </div>
                                <div class="summary-value text-danger">
                                    <asp:Label ID="lblTotalDeduction" runat="server" Text="0.00"></asp:Label>
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
                <asp:HyperLink ID="hlManageCards" runat="server"
                    NavigateUrl="~/lookup_management.aspx" CssClass="btn btn-outline-primary me-2">
                    <i class="fas fa-credit-card"></i> إدارة الكروت
                </asp:HyperLink>
            </div>

        </div>
    </div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <script type="text/javascript">
        function refreshPage() {
            // Force a full page reload
            location.reload(true);
        }

        function showSuccessMessage(message) {
            alert(message);
            // Reload the page after showing the message
            setTimeout(function () {
                location.reload(true);
            }, 500);
        }
    </script>
</asp:Content>
