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

        .btn-add-part {
            background-color: #28a745;
            border-color: #28a745;
            color: white;
            font-weight: bold;
            margin-top: 10px;
            height: 38px;
        }

        .btn-add-part:hover {
            background-color: #218838;
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

        .parts-container {
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 15px;
            margin-top: 10px;
            background-color: #f8f9fa;
        }

        .part-row {
            margin-bottom: 10px;
            display: flex;
            gap: 10px;
            align-items: center;
        }

        .part-row select {
            flex: 1;
        }

        .part-row button {
            width: 40px;
            background-color: #dc3545;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            padding: 6px;
        }

        .part-row button:hover {
            background-color: #c82333;
        }

        /* Engine Overhaul Styles */
        .engine-overhaul-card {
            background: linear-gradient(135deg, #fff8e1 0%, #ffe0b2 100%);
            border: 2px solid #ff9800 !important;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
        }

        .overhaul-status {
            padding: 10px;
            border-radius: 8px;
            font-weight: bold;
            text-align: center;
        }

        .overhaul-warning {
            background-color: #ffebee;
            border: 1px solid #f44336;
        }

        .overhaul-good {
            background-color: #e8f5e9;
            border: 1px solid #4caf50;
        }

        .overhaul-warning-text {
            color: #f44336;
            font-weight: bold;
        }

        .overhaul-info-text {
            color: #ff9800;
            font-weight: bold;
        }

        .overhaul-success-text {
            color: #4caf50;
            font-weight: bold;
        }

        .btn-overhaul {
            background-color: #ff9800;
            border-color: #ff9800;
            color: white;
            font-weight: bold;
            height: 45px;
        }

        .btn-overhaul:hover {
            background-color: #e68900;
            color: white;
        }

        .btn-clear-overhaul {
            background-color: #6c757d;
            border-color: #6c757d;
            color: white;
            font-weight: bold;
            height: 45px;
            margin-right: 10px;
        }

        .btn-clear-overhaul:hover {
            background-color: #5a6268;
            color: white;
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

<!-- Engine Overhaul Section - Add Cost field -->
<asp:Panel ID="pnlEngineOverhaul" runat="server" Visible="false">
    <div class="engine-overhaul-card">
        <div class="section-header" style="background: linear-gradient(90deg, #ff9800, #ffffff);">
            <i class="fas fa-oil-can"></i> عمرة الموتور
        </div>
        
        <div class="overhaul-status" id="divOverhaulStatus" runat="server" style="margin-bottom: 20px;">
            <asp:Label ID="lblOverhaulStatus" runat="server" Font-Bold="true" Font-Size="16px"></asp:Label>
        </div>
        
        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">تاريخ عمرة الموتور</label>
                <asp:TextBox ID="txtEngineOverhaulDate" runat="server" 
                    CssClass="form-control" 
                    TextMode="Date"></asp:TextBox>
                <small class="text-muted">اختر تاريخ إجراء العمرة</small>
            </div>
            
            <div class="col-md-3">
                <label class="form-label">قراءة العداد عند العمرة</label>
                <asp:TextBox ID="txtEngineOverhaulOdometer" runat="server" 
                    CssClass="form-control" 
                    TextMode="Number" 
                    step="1" 
                    min="0"
                    placeholder="كم"></asp:TextBox>
                <small class="text-muted">أدخل قراءة العداد عند إجراء العمرة</small>
            </div>
            
            <!-- NEW: Cost Field -->
            <div class="col-md-3">
                <label class="form-label">تكلفة العمرة</label>
                <asp:TextBox ID="txtEngineOverhaulCost" runat="server" 
                    CssClass="form-control" 
                    TextMode="Number" 
                    step="0.01" 
                    min="0"
                    placeholder="0.00"></asp:TextBox>
                <small class="text-muted">أدخل تكلفة عمرة الموتور</small>
            </div>
            
            <div class="col-md-3">
                <label class="form-label">ملاحظات العمرة</label>
                <asp:TextBox ID="txtEngineOverhaulNotes" runat="server" 
                    CssClass="form-control" 
                    TextMode="MultiLine" 
                    Rows="1"
                    placeholder="ملاحظات عن عمرة الموتور"></asp:TextBox>
            </div>
        </div>
        
        <div class="row mt-3">
            <div class="col-md-6">
                <asp:Button ID="btnSaveEngineOverhaul" runat="server" 
                    Text="تسجيل عمرة الموتور" 
                    CssClass="btn btn-overhaul w-100" 
                    OnClick="BtnSaveEngineOverhaul_Click" 
                    OnClientClick="return validateEngineOverhaul();" />
            </div>
            <div class="col-md-6">
                <asp:Button ID="btnClearEngineOverhaul" runat="server" 
                    Text="مسح بيانات العمرة" 
                    CssClass="btn btn-clear-overhaul w-100" 
                    OnClick="BtnClearEngineOverhaul_Click" 
                    OnClientClick="return confirm('هل أنت متأكد من مسح بيانات عمرة الموتور؟');" />
            </div>
        </div>
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
                            <asp:ListItem Value="صيانة دورية">صيانة دورية</asp:ListItem>
                            <asp:ListItem Value="اعطال">اعطال</asp:ListItem>
                            <asp:ListItem Value="صيانة طارئة">صيانة طارئة</asp:ListItem>
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

                    <div class="col-md-9">
                        <label class="form-label">ملاحظات</label>
                        <asp:TextBox ID="txtNotes" runat="server" 
                            CssClass="form-control" 
                            TextMode="MultiLine" 
                            Rows="2"
                            placeholder="ملاحظات إضافية عن الصيانة"></asp:TextBox>
                    </div>
                </div>

                <!-- Multiple Parts Selection -->
                <div class="row mt-3">
                    <div class="col-12">
                        <label class="form-label">قطع الغيار</label>
                        <div class="parts-container">
                            <div id="partRowsContainer">
                                <!-- Part rows will be added here dynamically -->
                            </div>
                            <button type="button" class="btn btn-sm btn-add-part" onclick="addPartRow()">
                                <i class="fas fa-plus"></i> إضافة قطعة غيار
                            </button>
                        </div>
                        <asp:HiddenField ID="hfPartsData" runat="server" />
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-md-6">
                        <asp:Button ID="btnSaveMaintenance" runat="server" 
                            Text="حفظ الصيانة" 
                            CssClass="btn btn-save" 
                            OnClick="BtnSaveMaintenance_Click" 
                           OnClientClick="return collectPartsData() && validateOdometerBeforeSave();" />
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
                <div class="col-md-6">
                    <div class="statistics-card">
                        <h5>إجمالي تكاليف الصيانة</h5>
                        <div class="statistics-value">
                            <asp:Label ID="lblTotalCost" runat="server" Text="0"></asp:Label> ج.م
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="statistics-card">
                        <h5>عدد مرات الصيانة</h5>
                        <div class="statistics-value" style="color: #007bff;">
                            <asp:Label ID="lblMaintenanceCount" runat="server" Text="0"></asp:Label>
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
                        <asp:BoundField DataField="PartsNames" HeaderText="قطع الغيار" />
                        <asp:BoundField DataField="OdometerReading" HeaderText="قراءة العداد" 
                            ItemStyle-CssClass="km-column" />
                        <asp:BoundField DataField="Cost" HeaderText="التكلفة" 
                            DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
                    </Columns>
                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                </asp:GridView>
            </asp:Panel>

        </div>

    </div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <script type="text/javascript">
        // Store parts data
        let availableParts = [];

        // Load parts from server-side data
        function loadPartsData() {
            <% if (ViewState["PartsData"] != null) { %>
            availableParts = <%= ViewState["PartsData"] %>;
            <% } %>
        }

        function addPartRow(partId = '', partName = '') {
            const container = document.getElementById('partRowsContainer');
            const rowDiv = document.createElement('div');
            rowDiv.className = 'part-row';
            rowDiv.style.display = 'flex';
            rowDiv.style.gap = '10px';
            rowDiv.style.marginBottom = '10px';

            const select = document.createElement('select');
            select.className = 'form-control';
            select.style.flex = '1';

            // Add empty option
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.text = '-- اختر قطعة غيار --';
            select.appendChild(emptyOption);

            // Add all parts options
            for (let i = 0; i < availableParts.length; i++) {
                const option = document.createElement('option');
                option.value = availableParts[i].id;
                option.text = availableParts[i].name;
                if (partId && availableParts[i].id == partId) {
                    option.selected = true;
                }
                select.appendChild(option);
            }

            const removeBtn = document.createElement('button');
            removeBtn.innerHTML = '<i class="fas fa-times"></i>';
            removeBtn.onclick = function () { removePartRow(this); };

            rowDiv.appendChild(select);
            rowDiv.appendChild(removeBtn);
            container.appendChild(rowDiv);
        }

        function removePartRow(btn) {
            btn.parentElement.remove();
        }

        function collectPartsData() {
            const partRows = document.querySelectorAll('#partRowsContainer .part-row');
            const partsIds = [];

            for (let i = 0; i < partRows.length; i++) {
                const select = partRows[i].querySelector('select');
                if (select && select.value) {
                    partsIds.push(select.value);
                }
            }

            // Store parts IDs in hidden field as comma-separated string
            const hfPartsData = document.getElementById('<%= hfPartsData.ClientID %>');
            if (hfPartsData) {
                hfPartsData.value = partsIds.join(',');
            }

            return true;
        }

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

        function validateOdometerBeforeSave() {
            var currentOdometerLabel = document.getElementById('<%= lblCurrentOdometer.ClientID %>');
            var newOdometerField = document.getElementById('<%= txtOdometerReading.ClientID %>');
    
            if (!currentOdometerLabel || !newOdometerField) {
                return true;
            }
    
            var currentOdometerText = currentOdometerLabel.innerText || currentOdometerLabel.textContent;
            var maxWorkOrderKM = parseInt(currentOdometerText.replace(/[^0-9]/g, ''), 10);
            var newOdo = parseInt(newOdometerField.value, 10);
    
            if (isNaN(newOdo) || newOdo <= 0) {
                alert('الرجاء إدخال قراءة عداد صحيحة أكبر من صفر');
                newOdometerField.style.backgroundColor = '#ffebee';
                newOdometerField.focus();
                return false;
            }
    
            if (newOdo > maxWorkOrderKM) {
                alert('خطأ: قراءة العداد المدخلة (' + newOdo.toLocaleString() + ' كم) لا يمكن أن تتجاوز آخر قراءة مسجلة في أوامر الشغل (' + maxWorkOrderKM.toLocaleString() + ' كم)');
                newOdometerField.style.backgroundColor = '#ffebee';
                newOdometerField.focus();
                return false;
            }
    
            newOdometerField.style.backgroundColor = '';
            return confirm('هل أنت متأكد من حفظ بيانات الصيانة؟\n\nقراءة العداد: ' + newOdo.toLocaleString() + ' كم');
        }

        function validateEngineOverhaul() {
            var overhaulDate = document.getElementById('<%= txtEngineOverhaulDate.ClientID %>');
            var overhaulOdometer = document.getElementById('<%= txtEngineOverhaulOdometer.ClientID %>');
            var currentOdometerLabel = document.getElementById('<%= lblCurrentOdometer.ClientID %>');
            
            if (!overhaulDate.value) {
                alert('الرجاء إدخال تاريخ عمرة الموتور');
                overhaulDate.focus();
                return false;
            }
            
            if (!overhaulOdometer.value || parseInt(overhaulOdometer.value) <= 0) {
                alert('الرجاء إدخال قراءة عداد صحيحة لعمرة الموتور');
                overhaulOdometer.focus();
                return false;
            }
            
            if (currentOdometerLabel) {
                var currentOdometerText = currentOdometerLabel.innerText || currentOdometerLabel.textContent;
                var currentKM = parseInt(currentOdometerText.replace(/[^0-9]/g, ''), 10);
                var overhaulKM = parseInt(overhaulOdometer.value, 10);
                
                if (overhaulKM > currentKM) {
                    alert('❌ خطأ: قراءة العداد المدخلة للعمرة (' + overhaulKM.toLocaleString() + ' كم) لا يمكن أن تتجاوز آخر قراءة مسجلة (' + currentKM.toLocaleString() + ' كم)');
                    overhaulOdometer.style.backgroundColor = '#ffebee';
                    overhaulOdometer.focus();
                    return false;
                }
            }
            
            overhaulOdometer.style.backgroundColor = '';
            return confirm('هل أنت متأكد من تسجيل عمرة الموتور؟\n\nالتاريخ: ' + overhaulDate.value + '\nالعداد: ' + parseInt(overhaulOdometer.value).toLocaleString() + ' كم');
        }
        function validateEngineOverhaul() {
            var overhaulDate = document.getElementById('<%= txtEngineOverhaulDate.ClientID %>');
            var overhaulOdometer = document.getElementById('<%= txtEngineOverhaulOdometer.ClientID %>');
    var currentOdometerLabel = document.getElementById('<%= lblCurrentOdometer.ClientID %>');
    
    if (!overhaulDate.value) {
        alert('الرجاء إدخال تاريخ عمرة الموتور');
        overhaulDate.focus();
        return false;
    }
    
    if (!overhaulOdometer.value || parseInt(overhaulOdometer.value) <= 0) {
        alert('الرجاء إدخال قراءة عداد صحيحة لعمرة الموتور');
        overhaulOdometer.focus();
        return false;
    }
    
    // Optional: Validate cost (allow empty or zero)
            var overhaulCost = document.getElementById('<%= txtEngineOverhaulCost.ClientID %>');
            if (overhaulCost && overhaulCost.value) {
                if (parseFloat(overhaulCost.value) < 0) {
                    alert('الرجاء إدخال تكلفة صحيحة (أكبر من أو تساوي صفر)');
                    overhaulCost.focus();
                    return false;
                }
            }

            if (currentOdometerLabel) {
                var currentOdometerText = currentOdometerLabel.innerText || currentOdometerLabel.textContent;
                var currentKM = parseInt(currentOdometerText.replace(/[^0-9]/g, ''), 10);
                var overhaulKM = parseInt(overhaulOdometer.value, 10);

                if (overhaulKM > currentKM) {
                    alert('❌ خطأ: قراءة العداد المدخلة للعمرة (' + overhaulKM.toLocaleString() + ' كم) لا يمكن أن تتجاوز آخر قراءة مسجلة (' + currentKM.toLocaleString() + ' كم)');
                    overhaulOdometer.style.backgroundColor = '#ffebee';
                    overhaulOdometer.focus();
                    return false;
                }
            }

            overhaulOdometer.style.backgroundColor = '';
            return confirm('هل أنت متأكد من تسجيل عمرة الموتور؟\n\nالتاريخ: ' + overhaulDate.value + '\nالعداد: ' + parseInt(overhaulOdometer.value).toLocaleString() + ' كم');
        }

        document.addEventListener('DOMContentLoaded', function() {
            var today = new Date().toISOString().split('T')[0];
            var maintDate = document.getElementById('<%= txtMaintenanceDate.ClientID %>');
            var overhaulDate = document.getElementById('<%= txtEngineOverhaulDate.ClientID %>');

            if (maintDate && !maintDate.value) {
                maintDate.value = today;
            }

            if (overhaulDate && !overhaulDate.value) {
                overhaulDate.value = today;
            }

            // Load parts data
            loadPartsData();

            // Add initial empty part row if no parts exist
            if (document.getElementById('partRowsContainer') && document.getElementById('partRowsContainer').children.length === 0) {
                addPartRow();
            }
        });
    </script>

</asp:Content>