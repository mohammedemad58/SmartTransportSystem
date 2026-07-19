<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="fuel_report.aspx.cs" Inherits="car_WebApplication1.fuel_report" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <title>تقرير استهلاك الوقود والكارت</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        /* Your existing styles */
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
            color:#a50d25;
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
            font-size: 24px;
            font-weight: bold;
            color: #343a40;
        }
        .text-primary { color: #0d6efd !important; }
        .text-danger { color: #dc3545 !important; }
        .text-success { color: #198754 !important; }
        .text-dark { color: #343a40 !important; }
        .fw-bold { font-weight: bold !important; }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4" dir="rtl">
        <div class="report-container">
            
            <h3 class="page-title text-center">
                <i class="fas fa-gas-pump"></i> تقرير استهلاك الوقود والكارت
            </h3>


            <div class="row mb-4 g-3">
                <div class="col-md-4">
                    <label class="form-label fw-bold">اختر السيارة</label>
                    <asp:DropDownList ID="ddlVehicle" runat="server" CssClass="form-select">
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

                <div class="col-md-2 d-flex align-items-end">
                    <asp:Button ID="btnSearch" runat="server" Text="بحث"
                        CssClass="btn btn-search w-100"
                        OnClick="btnSearch_Click" />
                </div>
            </div>

            <div class="table-responsive">
                <asp:GridView ID="gvFuel" runat="server"
                    CssClass="table table-bordered table-striped table-hover"
                    AutoGenerateColumns="true"
                    EmptyDataText="لا توجد نتائج للبحث"
                    EmptyDataRowStyle-CssClass="text-center text-muted p-5"
                    HeaderStyle-CssClass="table-header">
                </asp:GridView>
            </div>

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
</asp:Content>