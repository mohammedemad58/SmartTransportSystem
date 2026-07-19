<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="vehicle_maintenance_history.aspx.cs"
    Inherits="car_WebApplication1.vehicle_maintenance_history" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }
        
        .notes-column {
            max-width: 200px;
            white-space: normal;
            word-wrap: break-word;
            text-align: right;
            font-size: 13px;
            color: #666;
            padding: 8px !important;
        }

        .notes-column div {
            max-height: 60px;
            overflow-y: auto;
            padding: 5px;
            background-color: #f9f9f9;
            border-radius: 4px;
            border-right: 3px solid #c8102e;
        }

        .grid-view td {
            padding: 12px 8px;
            vertical-align: middle;
        }

        .grid-view th {
            background-color: #a50d25;
            color: white;
            font-weight: bold;
            padding: 12px;
            text-align: center;
            font-size: 14px;
        }

        .search-box {
            border: 2px solid #000;
            border-radius: 14px;
            padding: 25px;
            background-color: #fff;
            margin-bottom: 20px;
        }

        .section-header {
            background: linear-gradient(90deg, #ce1126, #ffffff);
            padding: 10px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-weight: bold;
            color: #000;
        }

        .form-label {
            font-weight: bold;
        }

        .btn-search {
            background-color: #a50d25;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-search:hover {
            background-color: #a50d25;
        }

        .btn-reset {
            background-color: #6c757d;
            color: #fff;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-reset:hover {
            background-color: #5a6268;
        }

        .car-number-container {
            display: flex;
            align-items: center;
            gap: 5px;
            flex-wrap: wrap;
            background: #f8f9fa;
            padding: 10px;
            border-radius: 8px;
            border: 1px solid #dee2e6;
        }

        .car-letter-box {
            width: 60px;
        }

        .car-number-single-box {
            width: 100px;
        }

        .car-letter, .car-number-single {
            text-align: center;
            font-size: 18px;
            font-weight: bold;
            direction: ltr;
        }

        .car-letter {
            background: #fff3f3;
            border-color: #ce1126;
        }

        .car-number-single {
            background: #f0f0f0;
            border-color: #333;
            letter-spacing: 2px;
        }

        .car-number-separator {
            font-size: 20px;
            font-weight: bold;
            color: #ce1126;
            margin: 0 5px;
        }

        .alert-card {
            border: 2px solid #ffc107;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 20px;
            background-color: #fff3cd;
        }

        .alert-header {
            color: #856404;
            font-weight: bold;
            font-size: 18px;
            margin-bottom: 10px;
        }

        .alert-item {
            background-color: white;
            border-right: 4px solid #dc3545;
            padding: 15px;
            margin-bottom: 10px;
            border-radius: 5px;
            transition: all 0.3s ease;
        }

        .alert-item-warning {
            border-right-color: #ffc107;
        }

        .alert-item-success {
            border-right-color: #28a745;
        }

        .badge-danger {
            background-color: #dc3545;
            color: white;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
        }

        .badge-warning {
            background-color: #ffc107;
            color: #212529;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
        }

        .badge-success {
            background-color: #28a745;
            color: white;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
        }

        .vehicle-info-card {
            background: #808080;
            color: white;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
        }

        .vehicle-info-item {
            text-align: center;
            padding: 10px;
        }

        .vehicle-info-label {
            font-size: 14px;
            opacity: 0.9;
        }

        .vehicle-info-value {
            font-size: 20px;
            font-weight: bold;
        }

        .grid-view {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }

        .grid-view th {
            background-color: #a50d25;
            color: white;
            font-weight: bold;
            padding: 12px;
            text-align: center;
        }

        .grid-view td {
            padding: 10px;
            border-bottom: 1px solid #dee2e6;
            text-align: center;
        }

        .grid-view tr:hover {
            background-color: #f5f5f5;
        }

        .grid-view .km-column {
            font-weight: bold;
            color: #28a745;
        }

        .grid-view .date-column {
            color: #007bff;
        }

        .grid-view .cost-column {
            color: #a50d25;
            font-weight: bold;
        }

        .current-odometer {
            font-size: 24px;
            font-weight: bold;
            color: #28a745;
        }

        .current-odometer-label {
            font-size: 20px;
            color: white;
        }

        .statistics-card {
            background-color: #f8f9fa;
            border-radius: 10px;
            padding: 15px;
            text-align: center;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .statistics-value {
            font-size: 24px;
            font-weight: bold;
            color: #c8102e;
        }

        .parts-list {
            max-width: 200px;
            font-size: 12px;
            text-align: right;
        }

        .parts-list ul {
            margin: 0;
            padding-right: 20px;
            list-style: none;
        }

        .parts-list li {
            margin: 2px 0;
            color: #495057;
        }

        @media (max-width: 768px) {
            .car-letter-box {
                width: 50px;
            }

            .car-number-single-box {
                width: 80px;
            }
             



            .alert-message-text {
    background-color: #fff3cd;
    border-radius: 5px;
    padding: 8px 12px;
    color: #856404;
    font-weight: bold;
    margin: 5px 0;
}

.alert-item {
    margin-bottom: 15px;
    padding: 15px;
    border-radius: 5px;
    transition: all 0.3s ease;
}

.alert-item-danger {
    border-right-color: #dc3545;
    background-color: #fff5f5;
}

.alert-item-warning {
    border-right-color: #ffc107;
    background-color: #fffbf0;
}

.alert-item-success {
    border-right-color: #28a745;
    background-color: #f0fff4;
}

.badge-danger {
    background-color: #dc3545;
    color: white;
    padding: 5px 10px;
    border-radius: 5px;
    font-size: 12px;
}

.badge-warning {
    background-color: #ffc107;
    color: #212529;
    padding: 5px 10px;
    border-radius: 5px;
    font-size: 12px;
}

.badge-success {
    background-color: #28a745;
    color: white;
    padding: 5px 10px;
    border-radius: 5px;
    font-size: 12px;
}
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <div class="text-center mb-4">
            <h2 class="page-title">سجل صيانة السيارة</h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" 
            CssClass="alert alert-danger text-center d-block" 
            Visible="false">
        </asp:Label>

        <asp:Label ID="lblSuccess" runat="server" 
            CssClass="alert alert-success text-center d-block" 
            Visible="false">
        </asp:Label>
        <!-- Success Alert for New Maintenance -->
<asp:Panel ID="pnlNewMaintenanceAlert" runat="server" Visible="false" CssClass="alert alert-success mb-3" style="border-right: 5px solid #28a745; direction: rtl;">
    <div class="d-flex align-items-start">
        <div class="flex-shrink-0">
            <i class="fas fa-check-circle fa-2x" style="color: #28a745;"></i>
        </div>
        <div class="flex-grow-1 me-3">
            <h5 class="mb-1">✅ تم إضافة صيانة جديدة بنجاح!</h5>
            <asp:Literal ID="litNewMaintenanceDetails" runat="server"></asp:Literal>
        </div>
        <button type="button" class="btn-close btn-close-white" onclick="this.parentElement.parentElement.style.display='none'"></button>
    </div>
</asp:Panel>
        <div class="search-box">
            <div class="section-header">
                <i class="fas fa-search"></i> بحث برقم السيارة
            </div>

            <div class="row">
                <div class="col-md-8">
                    <label class="form-label">رقم السيارة</label>
                    <div class="car-number-container mb-2">
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter1" runat="server"
                                CssClass="form-control car-letter"
                                MaxLength="1"
                                placeholder="أ"
                                onkeyup="moveToNext(this, 'txtCarLetter2');"
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, ''); }"
                                onkeypress="return isArabicLetter(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                        
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter2" runat="server"
                                CssClass="form-control car-letter"
                                MaxLength="1"
                                placeholder="ب"
                                onkeyup="moveToNext(this, 'txtCarLetter3');"
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter1'); }"
                                onkeypress="return isArabicLetter(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                        
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter3" runat="server"
                                CssClass="form-control car-letter"
                                MaxLength="1"
                                placeholder="ج"
                                onkeyup="moveToNext(this, 'txtCarNumbers');"
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter2'); }"
                                onkeypress="return isArabicLetter(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                        
                        <span class="car-number-separator">-</span>
                        
                        <div class="car-number-single-box">
                            <asp:TextBox ID="txtCarNumbers" runat="server"
                                CssClass="form-control car-number-single"
                                MaxLength="4"
                                placeholder="1234"
                                onkeypress="return isNumber(event);"
                                autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <label class="form-label">&nbsp;</label>
                    <div class="d-flex gap-2">
                        <asp:Button ID="btnSearch" runat="server" 
                            Text="بحث" 
                            CssClass="btn btn-search" 
                            OnClick="BtnSearch_Click" 
                            OnClientClick="return validateCarNumber();" />
                        <asp:Button ID="btnReset" runat="server" 
                            Text="إعادة تعيين" 
                            CssClass="btn btn-reset" 
                            OnClick="BtnReset_Click" />
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlVehicleInfo" runat="server" Visible="false">
            <div class="vehicle-info-card">
                <div class="row">
                    <div class="col-md-3 vehicle-info-item">
                        <div class="vehicle-info-label">رقم السيارة</div>
                        <div class="vehicle-info-value">
                            <asp:Label ID="lblCarNumber" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div class="col-md-3 vehicle-info-item">
                        <div class="vehicle-info-label">نوع السيارة</div>
                        <div class="vehicle-info-value">
                            <asp:Label ID="lblCarType" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div class="col-md-3 vehicle-info-item">
                        <div class="vehicle-info-label">الموديل</div>
                        <div class="vehicle-info-value">
                            <asp:Label ID="lblCarModel" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div class="col-md-3 vehicle-info-item">
                        <div class="vehicle-info-label">سنة الصنع</div>
                        <div class="vehicle-info-value">
                            <asp:Label ID="lblCarYear" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="row mt-3">
                    <div class="col-md-12 text-center">
                        <span class="current-odometer-label">قراءة العداد الحالية (أقصى قراءة):</span>
                        <span class="current-odometer">
                            <asp:Label ID="lblCurrentOdometer" runat="server" Text="0"></asp:Label> كم
                        </span>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlAlerts" runat="server" Visible="false">
            <div class="alert-card">
                <div class="alert-header">
                    <i class="fas fa-exclamation-triangle"></i> 
                    تنبيهات الصيانة
                </div>
                
    <asp:Repeater ID="rptAlerts" runat="server" OnItemDataBound="RptAlerts_ItemDataBound">
    <ItemTemplate>
        <div id="alertItem" runat="server" class="alert-item mb-3">
            <!-- Header row with service name and badge -->
            <div class="row mb-2">
                <div class="col-md-6">
                    <strong><i class="fas fa-wrench"></i> 
                        <asp:Label ID="lblServiceName" runat="server" Font-Size="16px"></asp:Label>
                    </strong>
                    <small class="text-muted me-2">
                        (<asp:Label ID="lblMaintenanceType" runat="server"></asp:Label>)
                    </small>
                </div>
                <div class="col-md-6 text-md-start">
                    <span id="badgeSpan" runat="server" class="badge">
                        <asp:Label ID="lblAlertMessage" runat="server"></asp:Label>
                    </span>
                </div>
            </div>
            
            <!-- Alert message row (full width) -->
            <div class="row mb-2">
                <div class="col-12">
                    <div class="alert-message-text">
                        <i class="fas fa-exclamation-triangle"></i>
                        <asp:Label ID="lblAlertMessageText" runat="server" Font-Weight="bold"></asp:Label>
                    </div>
                </div>
            </div>
            
            <!-- Two column layout for intervals and dates -->
            <div class="row">
                <div class="col-md-6">
                    <div class="row">
                        <div class="col-6">
                            <small class="text-muted">
                                <i class="fas fa-road"></i> المسافة: 
                                <asp:Label ID="lblKMInterval" runat="server" CssClass="fw-bold"></asp:Label>
                            </small>
                        </div>
                        <div class="col-6">
                            <small class="text-muted">
                                <i class="fas fa-clock"></i> الفترة الزمنية: 
                                <asp:Label ID="lblTimeInterval" runat="server" CssClass="fw-bold"></asp:Label>
                            </small>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="row">
                        <div class="col-6">
                            <small class="text-muted">
                                <i class="fas fa-calendar-alt"></i> آخر صيانة: 
                                <asp:Label ID="lblLastServiceDate" runat="server" CssClass="fw-bold"></asp:Label>
                            </small>
                        </div>
                        <div class="col-6">
                            <small class="text-muted">
                                <i class="fas fa-hourglass-half"></i> المتبقي: 
                                <asp:Label ID="lblRemainingDays" runat="server" CssClass="fw-bold"></asp:Label>
                            </small>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Bottom row with remaining KM and next due -->
            <div class="row mt-2">
                <div class="col-md-6">
                    <small class="text-muted">
                        <i class="fas fa-gauge-high"></i> المتبقي في المسافة: 
                        <asp:Label ID="lblRemainingKM" runat="server" CssClass="fw-bold"></asp:Label>
                    </small>
                </div>
                <div class="col-md-6">
                    <small class="text-info">
                        <i class="fas fa-calendar-alt"></i> الصيانة القادمة: 
                        <asp:Label ID="lblNextDue" runat="server" CssClass="fw-bold"></asp:Label>
                    </small>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
                
                <asp:Label ID="lblNoAlerts" runat="server" 
                    Text="لا توجد تنبيهات للصيانة" 
                    CssClass="text-success text-center d-block p-3"
                    Visible="false">
                </asp:Label>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlMaintenanceHistory" runat="server" Visible="false">
            <div class="section-header mt-4">
                <i class="fas fa-history"></i> سجل الصيانة السابقة
            </div>

            <asp:GridView ID="gvMaintenanceHistory" runat="server" 
                CssClass="grid-view" 
                AutoGenerateColumns="False" 
                EmptyDataText="لا توجد سجلات صيانة لهذه السيارة"
                ShowHeaderWhenEmpty="True"
                AllowPaging="True"
                PageSize="10"
                OnPageIndexChanging="GvMaintenanceHistory_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="ServiceDate" HeaderText="تاريخ الصيانة" 
                        DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="False" />
                    <asp:BoundField DataField="MaintenanceType" HeaderText="نوع الصيانة" />
                    <asp:TemplateField HeaderText="قطع الغيار">
                        <ItemTemplate>
                            <div class="parts-list">
                                <asp:Literal ID="litParts" runat="server" 
                                    Text='<%# GetMaintenanceParts(Eval("HistoryID")) %>' />
                            </div>
                        </ItemTemplate>
                        <ItemStyle CssClass="notes-column" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="OdometerReading" HeaderText="قراءة العداد (كم)" 
                        ItemStyle-CssClass="km-column" />
                    <asp:BoundField DataField="Cost" HeaderText="التكلفة" 
                        DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
                    <asp:TemplateField HeaderText="ملاحظات">
                        <ItemTemplate>
                            <div style="max-width: 200px; text-align: right;">
                                <%# Eval("Notes") != DBNull.Value && !string.IsNullOrEmpty(Eval("Notes").ToString()) ? Eval("Notes") : "—" %>
                            </div>
                        </ItemTemplate>
                        <ItemStyle CssClass="notes-column" />
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlStatistics" runat="server" Visible="false" CssClass="row mt-4">
            <div class="col-md-4">
                <div class="statistics-card">
                    <h5 class="card-title">إجمالي تكاليف الصيانة</h5>
                    <div class="statistics-value">
                        <asp:Label ID="lblTotalCost" runat="server" Text="0"></asp:Label> ج.م
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="statistics-card">
                    <h5 class="card-title">عدد مرات الصيانة</h5>
                    <div class="statistics-value" style="color: #007bff;">
                        <asp:Label ID="lblMaintenanceCount" runat="server" Text="0"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="statistics-card">
                    <h5 class="card-title">آخر قراءة عداد (أقصى قراءة)</h5>
                    <div class="statistics-value" style="color: #28a745;">
                        <asp:Label ID="lblLastOdometer" runat="server" Text="0"></asp:Label> كم
                    </div>
                </div>
            </div>
        </asp:Panel>

    </div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <script type="text/javascript">
        function moveToNext(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) {
                    nextField.focus();
                }
            }
        }

        function moveToPrevious(current, prevFieldId) {
            if (current.value.length === 0 && prevFieldId) {
                var prevField = document.getElementById(prevFieldId);
                if (prevField) {
                    prevField.focus();
                }
            }
        }

        function isArabicLetter(event) {
            var charCode = event.which || event.keyCode;
            var charStr = String.fromCharCode(charCode);
            var arabicPattern = /[\u0600-\u06FF]/;

            if (charCode === 8 || charCode === 9 || charCode === 13 ||
                charCode === 37 || charCode === 39 || charCode === 46) {
                return true;
            }
            return arabicPattern.test(charStr);
        }

        function isNumber(event) {
            var charCode = event.which || event.keyCode;

            if (charCode >= 48 && charCode <= 57) return true;
            if (charCode === 8 || charCode === 9 || charCode === 13 ||
                charCode === 37 || charCode === 39 || charCode === 46) return true;

            var arabicNumberPattern = /[\u0660-\u0669]/;
            return arabicNumberPattern.test(String.fromCharCode(charCode));
        }

        function validateCarNumber() {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtCarLetter2.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');

            if (!letter1 || !letter2 || !numbers) {
                alert('خطأ في عناصر الإدخال');
                return false;
            }

            if (!letter1.value || !letter2.value || !numbers.value) {
                alert('الرجاء إدخال رقم السيارة كاملاً');
                return false;
            }

            return true;
        }
    </script>

</asp:Content>