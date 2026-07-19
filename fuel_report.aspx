<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="fuel_report.aspx.cs" Inherits="car_WebApplication1.fuel_report" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <title>تقرير استهلاك الوقود والكارت</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f8f9fa;
        }
        .report-container {
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
        .btn-search {
            background-color: #a50d25;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 38px;
        }
        .btn-search:hover {
            background-color: #a50d25;
            border-color: #a50d25;
        }
        .btn-export {
            background-color: #198754;
            border-color: #198754;
            color: white;
            font-weight: bold;
            height: 38px;
        }
        .btn-export:hover {
            background-color: #157347;
            border-color: #146c43;
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
            transition: transform 0.2s;
        }
        .summary-item:hover {
            transform: translateY(-3px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }
        .summary-label {
            font-size: 14px;
            color: #6c757d;
            margin-bottom: 5px;
        }
        .summary-value {
            font-size: 24px;
            font-weight: bold;
            color: #343a40;
        }
        .text-primary { color: #0d6efd !important; }
        .text-danger { color: #dc3545 !important; }
        .text-success { color: #198754 !important; }
        .text-dark { color: #343a40 !important; }
        .fw-bold { font-weight: bold !important; }
        .consumption-rate {
            font-size: 18px;
            font-weight: bold;
            color: #a50d25;
        }
        .tab-content {
            padding: 20px 0;
        }
        .nav-tabs .nav-link {
            color: #495057;
            font-weight: bold;
        }
        .nav-tabs .nav-link.active {
            color: #a50d25;
            border-bottom: 3px solid #c8102e;
        }
        .card-stats {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 10px;
            padding: 15px;
            margin: 10px 0;
        }
        .badge-natural-gas {
            background-color: #17a2b8;
            color: white;
            padding: 3px 8px;
            border-radius: 12px;
            font-size: 11px;
        }
        .alert-info-custom {
            background-color: #e7f3ff;
            border-right: 4px solid #17a2b8;
            padding: 10px;
            margin-top: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4" dir="rtl">
        <div class="report-container">
            
            <h3 class="page-title text-center">
                <i class="fas fa-gas-pump"></i> تقرير استهلاك الوقود والكارت
            </h3>

            <!-- Search Filters -->
            <div class="row mb-4 g-3">
                <div class="col-md-4">
                    <label class="form-label fw-bold">اختر السيارة</label>
                    <asp:DropDownList ID="ddlVehicle" runat="server" CssClass="form-select" AutoPostBack="false">
                        <asp:ListItem Text="-- اختر السيارة --" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-bold">من تاريخ</label>
                    <asp:TextBox ID="txtFrom" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-bold">إلى تاريخ</label>
                    <asp:TextBox ID="txtTo" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-md-2 d-flex gap-2 align-items-end">
                    <asp:Button ID="btnSearch" runat="server" Text="بحث"
                        CssClass="btn btn-search w-100"
                        OnClick="btnSearch_Click" />
                </div>
            </div>

            <!-- Export Button Row -->
            <div class="row mb-3">
                <div class="col-md-12 text-start">
                    <asp:Button ID="btnExportExcel" runat="server" Text="تصدير إلى Excel" 
                        CssClass="btn btn-export" OnClick="btnExportExcel_Click" 
                        Visible="false" />
                </div>
            </div>

            <!-- Tabs for different reports -->
            <ul class="nav nav-tabs" id="reportTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link active" id="fuel-tab" data-bs-toggle="tab" data-bs-target="#fuel" type="button" role="tab">
                        <i class="fas fa-oil-can"></i> تقرير الوقود
                    </button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link" id="card-tab" data-bs-toggle="tab" data-bs-target="#card" type="button" role="tab">
                        <i class="fas fa-credit-card"></i> معاملات الكروت
                    </button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link" id="analysis-tab" data-bs-toggle="tab" data-bs-target="#analysis" type="button" role="tab">
                        <i class="fas fa-chart-line"></i> تحليل الاستهلاك
                    </button>
                </li>
            </ul>

            <div class="tab-content mt-3">
                <!-- Fuel Report Tab -->
                <div class="tab-pane fade show active" id="fuel" role="tabpanel">
                    <div class="table-responsive">
                        <asp:GridView ID="gvFuel" runat="server"
                            CssClass="table table-bordered table-striped table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد نتائج للبحث"
                            EmptyDataRowStyle-CssClass="text-center text-muted p-5"
                            HeaderStyle-CssClass="table-header"
                            OnRowDataBound="gvFuel_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="رقم_المعاملة" HeaderText="رقم المعاملة" />
                                <asp:BoundField DataField="التاريخ" HeaderText="التاريخ" />
                                <asp:BoundField DataField="نوع_الوقود" HeaderText="نوع الوقود" />
                                <asp:BoundField DataField="اللترات" HeaderText="اللترات" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="سعر_اللتر" HeaderText="سعر اللتر" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="عدد_الأمتار_المكعبة" HeaderText="عدد الأمتار المكعبة (م³)" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="سعر_المتر_المكعب" HeaderText="سعر المتر المكعب" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="التكلفة" HeaderText="التكلفة" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="المسافة" HeaderText="المسافة (كم)" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="معدل_الاستهلاك" HeaderText="معدل الاستهلاك كيلومتر لكل لتر" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="المحطة" HeaderText="المحطة" />
                                <asp:BoundField DataField="ملاحظات" HeaderText="ملاحظات" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

                <!-- Card Transactions Tab -->
                <div class="tab-pane fade" id="card" role="tabpanel">
                    <div class="table-responsive">
                        <asp:GridView ID="gvCardTransactions" runat="server"
                            CssClass="table table-bordered table-striped table-hover"
                            AutoGenerateColumns="false"
                            EmptyDataText="لا توجد معاملات كارت"
                            EmptyDataRowStyle-CssClass="text-center text-muted p-5"
                            HeaderStyle-CssClass="table-header">
                            <Columns>
                                <asp:BoundField DataField="رقم_المعاملة" HeaderText="رقم المعاملة" />
                                <asp:BoundField DataField="التاريخ" HeaderText="التاريخ" />
                                <asp:BoundField DataField="رقم_الكارت" HeaderText="رقم الكارت" />
                                <asp:BoundField DataField="نوع_المعاملة" HeaderText="نوع المعاملة" />
                                <asp:BoundField DataField="المبلغ" HeaderText="المبلغ" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="السيارة" HeaderText="السيارة" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    
                    <!-- Card Summary Stats -->
                    <div class="row mt-4">
                        <div class="col-md-4">
                            <div class="card-stats text-center">
                                <i class="fas fa-arrow-up fa-2x"></i>
                                <h6>إجمالي الإيداعات</h6>
                                <h4><asp:Label ID="lblCardDeposits" runat="server" Text="0.00"></asp:Label> ج</h4>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="card-stats text-center">
                                <i class="fas fa-arrow-down fa-2x"></i>
                                <h6>إجمالي المسحوبات</h6>
                                <h4><asp:Label ID="lblCardWithdrawals" runat="server" Text="0.00"></asp:Label> ج</h4>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="card-stats text-center">
                                <i class="fas fa-balance-scale fa-2x"></i>
                                <h6>رصيد الكارت</h6>
                                <h4><asp:Label ID="lblCardBalance" runat="server" Text="0.00"></asp:Label> ج</h4>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Analysis Tab -->
                <div class="tab-pane fade" id="analysis" role="tabpanel">
             
          

                    <div class="row">
                        <div class="col-md-4">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-tachometer-alt"></i> متوسط الاستهلاك
                                </div>
                                <div class="consumption-rate">
                                    <asp:Label ID="lblAvgConsumption" runat="server" Text="0.00"></asp:Label>
                                </div>
                                <small><asp:Label ID="lblConsumptionUnit" runat="server" Text="كم/لتر"></asp:Label></small>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-chart-line"></i> أعلي معدل استهلاك
                                </div>
                                <div class="summary-value text-success">
                                    <asp:Label ID="lblBestConsumption" runat="server" Text="0.00"></asp:Label>
                                    <small>كم/لتر</small>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-chart-line"></i> أدني معدل استهلاك
                                </div>
                                <div class="summary-value text-danger">
                                    <asp:Label ID="lblWorstConsumption" runat="server" Text="0.00"></asp:Label>
                                    <small>كم/لتر</small>
                                </div>
                            </div>
                        </div>
                    </div>

       

                    <!-- Natural Gas Consumption Section -->
                    <div class="row mt-3">
                        <div class="col-md-12">
                            <div class="summary-item">
                                <div class="summary-label">
                                    <i class="fas fa-chart-pie"></i> تحليل استهلاك الغاز الطبيعي
                                </div>
                                <div class="row mt-3">
                                    <div class="col-md-6">
                                        <div class="text-center">
                                            <h6>إجمالي الأمتار المكعبة المستهلكة</h6>
                                            <h3 class="text-info"><asp:Label ID="lblTotalCubicMeters" runat="server" Text="0.00"></asp:Label> م³</h3>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="text-center">
                                            <h6>إجمالي تكلفة الغاز الطبيعي</h6>
                                            <h3 class="text-success"><asp:Label ID="lblTotalGasCost" runat="server" Text="0.00"></asp:Label> ج</h3>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Summary Statistics (Always visible) -->
            <hr />
            <div class="row text-center mt-4">
                <div class="col-md-3">
                    <div class="summary-item">
                        <div class="summary-label">
                            <i class="fas fa-oil-can"></i> إجمالي اللترات
                        </div>
                        <div class="summary-value">
                            <asp:Label ID="lblTotalLiters" runat="server" CssClass="fw-bold text-primary" Text="0.00"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="summary-item">
                        <div class="summary-label">
                            <i class="fas fa-money-bill"></i> إجمالي التكلفة
                        </div>
                        <div class="summary-value">
                            <asp:Label ID="lblTotalCost" runat="server" CssClass="fw-bold text-danger" Text="0.00"></asp:Label> ج
                        </div>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="summary-item">
                        <div class="summary-label">
                            <i class="fas fa-credit-card"></i> مصروف الكارت
                        </div>
                        <div class="summary-value">
                            <asp:Label ID="lblCardAmount" runat="server" CssClass="fw-bold text-success" Text="0.00"></asp:Label> ج
                        </div>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="summary-item">
                        <div class="summary-label">
                            <i class="fas fa-chart-line"></i> الفرق
                        </div>
                        <div class="summary-value">
                            <asp:Label ID="lblDifference" runat="server" CssClass="fw-bold text-dark" Text="0.00"></asp:Label> ج
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
</asp:Content>