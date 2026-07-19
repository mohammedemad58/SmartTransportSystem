<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="car_maintenance.aspx.cs"
    Inherits="car_WebApplication1.car_maintenance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }

        .maintenance-box {
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

        .btn-save {
            background-color: #a50d25;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-save:hover {
            background-color: #a50d25;
        }

        .btn-view-history {
            background-color: #17a2b8;
            border-color: #17a2b8;
            color: white;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-view-history:hover {
            background-color: #138496;
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

        .car-info-card {
            background: #808080;
            color: white;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 20px;
        }

        .car-number-display {
            font-size: 24px;
            font-weight: bold;
            text-align: center;
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

        .maintenance-history {
            margin-top: 30px;
        }

        .grid-view {
            width: 100%;
            border-collapse: collapse;
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

        .grid-view .cost-column {
            color: #a50d25;
            font-weight: bold;
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

        .badge {
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
            font-weight: bold;
            display: inline-block;
        }

        .badge-danger {
            background-color: #dc3545;
            color: white;
        }

        .badge-warning {
            background-color: #ffc107;
            color: #212529;
        }

        .badge-success {
            background-color: #28a745;
            color: white;
        }

        .current-odometer {
            background-color: #e8f5e9;
            border: 2px solid #28a745;
            border-radius: 10px;
            padding: 15px;
            margin-top: 10px;
            margin-bottom: 20px;
            text-align: center;
        }

        .current-odometer-label {
            font-size: 16px;
            color: #28a745;
            font-weight: bold;
        }

        .current-odometer-value {
            font-size: 24px;
            font-weight: bold;
            color: #28a745;
        }

        .statistics-card {
            background-color: #f8f9fa;
            border-radius: 10px;
            padding: 15px;
            text-align: center;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 15px;
        }

        .statistics-value {
            font-size: 24px;
            font-weight: bold;
            color: #c8102e;
        }

        @media (max-width: 768px) {
            .car-letter-box {
                width: 50px;
            }

            .car-number-single-box {
                width: 80px;
            }
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <div class="text-center mb-4">
            <h2 class="page-title">بيانات الصيانة</h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" 
            CssClass="alert alert-danger text-center d-block" 
            Visible="false">
        </asp:Label>

        <asp:Label ID="lblSuccess" runat="server" 
            CssClass="alert alert-success text-center d-block" 
            Visible="false">
        </asp:Label>

        <div class="maintenance-box">

            <div class="section-header">
                <i class="fas fa-search"></i> بحث برقم السيارة
            </div>

            <div class="row mb-4">
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
                    <asp:Button ID="btnSearch" runat="server" 
                        Text="بحث" 
                        CssClass="btn btn-search w-100" 
                        OnClick="BtnSearch_Click" 
                        OnClientClick="return validateCarNumber();" />
                </div>
            </div>

            <asp:Panel ID="pnlCarInfo" runat="server" Visible="false">
                <div class="car-info-card">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="car-number-display">
                                رقم السيارة: <asp:Label ID="lblCarNumber" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlCurrentOdometer" runat="server" Visible="false">
                <div class="current-odometer">
                    <span class="current-odometer-label">قراءة العداد الحالية (أقصى قراءة):</span>
                    <span class="current-odometer-value">
                        <asp:Label ID="lblCurrentOdometer" runat="server" Text="0"></asp:Label> كم
                    </span>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlMaintenanceForm" runat="server" Visible="false">
                <div class="section-header">
                    <i class="fas fa-wrench"></i> إضافة صيانة جديدة
                </div>

                <div class="row g-3">
                    <div class="col-md-4">
                        <label class="form-label">نوع الصيانة</label>
                        <asp:DropDownList ID="ddlMaintenanceType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر نوع الصيانة --</asp:ListItem>
                            <asp:ListItem Value="دورية">صيانة دورية</asp:ListItem>
                            <asp:ListItem Value="إصلاح">إصلاح</asp:ListItem>
                            <asp:ListItem Value="استبدال زيوت">استبدال زيوت</asp:ListItem>
                            <asp:ListItem Value="إطارات">إطارات</asp:ListItem>
                            <asp:ListItem Value="فرامل">فرامل</asp:ListItem>
                            <asp:ListItem Value="كهرباء">كهرباء</asp:ListItem>
                            <asp:ListItem Value="ميكانيكا">ميكانيكا</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">تاريخ الصيانة</label>
                        <asp:TextBox ID="txtMaintenanceDate" runat="server" 
                            CssClass="form-control" 
                            TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">قراءة العداد</label>
                        <asp:TextBox ID="txtOdometerReading" runat="server" 
                            CssClass="form-control" 
                            TextMode="Number" 
                            step="1" 
                            min="0"
                            placeholder="كم"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">التكلفة</label>
                        <asp:TextBox ID="txtCost" runat="server" 
                            CssClass="form-control" 
                            TextMode="Number" 
                            step="0.01" 
                            min="0"
                            placeholder="0.00"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">الصيانة القادمة (كم)</label>
                        <asp:TextBox ID="txtNextMaintenanceKM" runat="server" 
                            CssClass="form-control" 
                            TextMode="Number" 
                            step="1" 
                            min="0"
                            placeholder="5000"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">الصيانة القادمة (تاريخ)</label>
                        <asp:TextBox ID="txtNextMaintenanceDate" runat="server" 
                            CssClass="form-control" 
                            TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">نوع الخدمة</label>
                        <asp:DropDownList ID="ddlServiceType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر نوع الخدمة --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">قطعة الغيار</label>
                        <asp:DropDownList ID="ddlParts" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر قطعة غيار --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-9">
                        <label class="form-label">ملاحظات</label>
                        <asp:TextBox ID="txtNotes" runat="server" 
                            CssClass="form-control" 
                            TextMode="MultiLine" 
                            Rows="2"
                            placeholder="ملاحظات إضافية عن الصيانة"></asp:TextBox>
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-md-6">
                        <asp:Button ID="btnSaveMaintenance" runat="server" 
                            Text="حفظ الصيانة" 
                            CssClass="btn btn-save" 
                            OnClick="BtnSaveMaintenance_Click" 
                           OnClientClick="return validateOdometerBeforeSave();" />
                    </div>
                    <div class="col-md-6">
                        <asp:Button ID="btnViewHistory" runat="server" 
                            Text="عرض سجل الصيانة الكامل" 
                            CssClass="btn btn-view-history" 
                            OnClick="BtnViewHistory_Click" />
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlStatistics" runat="server" Visible="false" CssClass="row mt-4">
                <div class="col-md-4">
                    <div class="statistics-card">
                        <h5>إجمالي تكاليف الصيانة</h5>
                        <div class="statistics-value">
                            <asp:Label ID="lblTotalCost" runat="server" Text="0"></asp:Label> ج.م
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="statistics-card">
                        <h5>عدد مرات الصيانة</h5>
                        <div class="statistics-value" style="color: #007bff;">
                            <asp:Label ID="lblMaintenanceCount" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="statistics-card">
                        <h5>آخر قراءة عداد (أقصى قراءة)</h5>
                        <div class="statistics-value" style="color: #28a745;">
                            <asp:Label ID="lblLastOdometer" runat="server" Text="0"></asp:Label> كم
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlMaintenanceHistory" runat="server" Visible="false" CssClass="maintenance-history">
                <div class="section-header">
                    <i class="fas fa-history"></i> سجل الصيانة السابقة
                </div>

                <asp:GridView ID="gvMaintenanceHistory" runat="server" 
                    CssClass="grid-view" 
                    AutoGenerateColumns="False" 
                    EmptyDataText="لا توجد سجلات صيانة لهذه السيارة"
                    ShowHeaderWhenEmpty="True"
                    AllowPaging="True"
                    PageSize="5"
                    OnPageIndexChanging="GvMaintenanceHistory_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="ServiceDate" HeaderText="تاريخ الصيانة" 
                            DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="False" />
                        <asp:BoundField DataField="MaintenanceType" HeaderText="نوع الصيانة" />
                        <asp:BoundField DataField="ServiceName" HeaderText="نوع الخدمة" />
                        <asp:BoundField DataField="OdometerReading" HeaderText="قراءة العداد" 
                            ItemStyle-CssClass="km-column" />
                        <asp:BoundField DataField="Cost" HeaderText="التكلفة" 
                            DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
                        <asp:BoundField DataField="NextDueKM" HeaderText="الصيانة القادمة (كم)" 
                            ItemStyle-CssClass="km-column" />
                        <asp:BoundField DataField="NextDueDate" HeaderText="تاريخ الصيانة القادمة" 
                            DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                </asp:GridView>
            </asp:Panel>

        </div>

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

            if (!letter1 || !letter2 || !numbers) return false;

            if (!letter1.value || !letter2.value || !numbers.value) {
                alert('الرجاء إدخال رقم السيارة كاملاً');
                return false;
            }

            return true;
        }

        function validateOdometer() {
            var currentOdometerText = document.getElementById('<%= lblCurrentOdometer.ClientID %>').innerText;
            var newOdometer = document.getElementById('<%= txtOdometerReading.ClientID %>').value;
            
            var currentOdometer = parseInt(currentOdometerText.replace(/[^0-9]/g, ''), 10);
            var newOdo = parseInt(newOdometer, 10);
            
            if (isNaN(newOdo) || newOdo <= 0) {
                alert('الرجاء إدخال قراءة عداد صحيحة');
                return false;
            }
            
            if (newOdo < currentOdometer) {
                alert('تحذير: قراءة العداد الجديدة (' + newOdo + ' كم) أقل من القراءة الحالية (' + currentOdometer + ' كم)');
                return false;
            }
            
            return confirm('هل أنت متأكد من حفظ بيانات الصيانة؟\nقراءة العداد الجديدة: ' + newOdo + ' كم');
        }

        document.addEventListener('DOMContentLoaded', function() {
            var today = new Date().toISOString().split('T')[0];
            var maintDate = document.getElementById('<%= txtMaintenanceDate.ClientID %>');
            var nextDate = document.getElementById('<%= txtNextMaintenanceDate.ClientID %>');

            if (maintDate && !maintDate.value) {
                maintDate.value = today;
            }

            if (nextDate && !nextDate.value) {
                var next = new Date();
                next.setMonth(next.getMonth() + 3);
                nextDate.value = next.toISOString().split('T')[0];
            }
        });
        function validateOdometerBeforeSave() {
            // Get the current odometer value from the label (this is the work_order max km)
            var currentOdometerLabel = document.getElementById('<%= lblCurrentOdometer.ClientID %>');
    var newOdometerField = document.getElementById('<%= txtOdometerReading.ClientID %>');
    
    if (!currentOdometerLabel || !newOdometerField) {
        return true;
    }
    
    // Extract number from label (remove commas and 'كم' text)
    var currentOdometerText = currentOdometerLabel.innerText || currentOdometerLabel.textContent;
    var maxWorkOrderKM = parseInt(currentOdometerText.replace(/[^0-9]/g, ''), 10);
    var newOdo = parseInt(newOdometerField.value, 10);
    
    if (isNaN(newOdo) || newOdo <= 0) {
        alert('الرجاء إدخال قراءة عداد صحيحة أكبر من صفر');
        newOdometerField.style.backgroundColor = '#ffebee';
        newOdometerField.focus();
        return false;
    }
    
    // NEW VALIDATION: New odometer must be LESS THAN OR EQUAL to work_order km
    if (newOdo > maxWorkOrderKM) {
        alert('خطأ: قراءة العداد المدخلة (' + newOdo.toLocaleString() + ' كم) لا يمكن أن تتجاوز آخر قراءة مسجلة في أوامر الشغل (' + maxWorkOrderKM.toLocaleString() + ' كم)');
        newOdometerField.style.backgroundColor = '#ffebee';
        newOdometerField.focus();
        return false;
    }
    
    // Reset background color if validation passes
    newOdometerField.style.backgroundColor = '';
    
    return confirm('هل أنت متأكد من حفظ بيانات الصيانة؟\n\nقراءة العداد: ' + newOdo.toLocaleString() + ' كم');
}

// Real-time validation
function validateOdometerRealtime() {
    var currentOdometerLabel = document.getElementById('<%= lblCurrentOdometer.ClientID %>');
    var newOdometerField = document.getElementById('<%= txtOdometerReading.ClientID %>');

    if (!currentOdometerLabel || !newOdometerField) return;

    var currentOdometerText = currentOdometerLabel.innerText || currentOdometerLabel.textContent;
    var maxWorkOrderKM = parseInt(currentOdometerText.replace(/[^0-9]/g, ''), 10);
    var newOdo = parseInt(newOdometerField.value, 10);

    if (!isNaN(newOdo) && newOdo > 0 && maxWorkOrderKM > 0) {
        if (newOdo > maxWorkOrderKM) {
            newOdometerField.style.backgroundColor = '#ffebee';
            newOdometerField.style.borderColor = '#dc3545';
        } else {
            newOdometerField.style.backgroundColor = '#e8f5e9';
            newOdometerField.style.borderColor = '#28a745';
        }
    } else {
        newOdometerField.style.backgroundColor = '';
        newOdometerField.style.borderColor = '';
    }
}
    </script>

</asp:Content>