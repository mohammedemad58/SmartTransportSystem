<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="car_insurance.aspx.cs"
    Inherits="car_WebApplication1.car_insurance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }

        .insurance-box {
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

        .btn-back {
            background-color: #6c757d;
            color: #fff;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-back:hover {
            background-color: #5a6268;
        }
        
        .btn-search {
            background-color: #a50d25;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 50px;
            width: 100%;
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

        .insurance-history {
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

        .status-active {
            color: #28a745;
            font-weight: bold;
        }

        .status-expired {
            color: #dc3545;
            font-weight: bold;
        }

        .insurance-type-badge {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: bold;
        }
        
        .mandatory-insurance {
            background-color: #ffc107;
            color: #000;
        }
        
        .comprehensive-insurance {
            background-color: #17a2b8;
            color: #fff;
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
            <h2 class="page-title"> بيانات التأمين التكاملي</h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" 
            CssClass="alert alert-danger text-center d-block" 
            Visible="false">
        </asp:Label>

        <asp:Label ID="lblSuccess" runat="server" 
            CssClass="alert alert-success text-center d-block" 
            Visible="false">
        </asp:Label>

        <div class="insurance-box">

            <!-- قسم البحث بالرقم -->
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
                        OnClick="btnSearch_Click" 
                        OnClientClick="return validateCarNumber();" />
                </div>
            </div>

            <!-- معلومات السيارة -->
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

            <!-- نموذج التأمين الشامل (التأملي) -->
            <asp:Panel ID="pnlInsuranceForm" runat="server" Visible="false">
                <div class="section-header">
                    <i class="fas fa-shield-alt"></i> إضافة تأمين تكاملي (تكاملي)
                </div>

                <div class="row g-3">
                    <div class="col-md-6">
    <label class="form-label">شركة التأمين <span class="text-danger">*</span></label>
    <div class="input-group">
        <asp:DropDownList ID="ddlInsuranceCompany" runat="server" CssClass="form-control">
            <asp:ListItem Value="">-- اختر شركة التأمين --</asp:ListItem>
        </asp:DropDownList>
    </div>
</div>
                    <div class="col-md-6">
                        <label class="form-label">التكلفة <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">تاريخ البداية <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">تاريخ النهاية <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">ملاحظات</label>
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-md-6">
                        <asp:Button ID="btnSaveInsurance" runat="server" 
                            Text="حفظ التأمين التكاملي" 
                            CssClass="btn btn-save" 
                            OnClick="btnSaveInsurance_Click" />
                    </div>
                    <div class="col-md-6">
                        <asp:Button ID="btnBack" runat="server" 
                            Text="رجوع" 
                            CssClass="btn btn-back" 
                            OnClick="btnBack_Click" />
                    </div>
                </div>
            </asp:Panel>

            <!-- سجل التأمينات السابقة -->
            <asp:Panel ID="pnlInsuranceHistory" runat="server" Visible="false" CssClass="insurance-history">
                <div class="section-header">
                    <i class="fas fa-history"></i> سجل التأمينات السابقة
                </div>

                <asp:GridView ID="gvInsuranceHistory" runat="server" 
                    CssClass="grid-view" 
                    AutoGenerateColumns="False" 
                    EmptyDataText="لا توجد تأمينات سابقة"
                    ShowHeaderWhenEmpty="True"
                    AllowPaging="True"
                    PageSize="5"
                    OnPageIndexChanging="gvInsuranceHistory_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="company_name" HeaderText="الشركة" />
                        <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="cost" HeaderText="التكلفة" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="notes" HeaderText="ملاحظات" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" 
                                    CssClass='<%# Convert.ToDateTime(Eval("end_date")) > DateTime.Now ? "status-active" : "status-expired" %>'
                                    Text='<%# Convert.ToDateTime(Eval("end_date")) > DateTime.Now ? "ساري" : "منتهي" %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                </asp:GridView>
            </asp:Panel>

        </div>

    </div>

    <!-- Font Awesome -->
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

        // تعيين التواريخ الافتراضية
        document.addEventListener('DOMContentLoaded', function() {
            var today = new Date().toISOString().split('T')[0];
            var startDate = document.getElementById('<%= txtStartDate.ClientID %>');
            var endDate = document.getElementById('<%= txtEndDate.ClientID %>');

            if (startDate && !startDate.value) {
                startDate.value = today;
            }

            if (endDate && !endDate.value) {
                var next = new Date();
                next.setFullYear(next.getFullYear() + 1);
                endDate.value = next.toISOString().split('T')[0];
            }
        });

        // Function to open insurance companies lookup
        function openInsuranceLookup() {
            var url = 'lookup_management.aspx?tab=insuranceCompanies';
            var features = 'width=900,height=650,resizable=yes,scrollbars=yes';
            var lookupWindow = window.open(url, 'LookupManagement', features);

            // Set up a timer to check when popup closes
            var checkClosed = setInterval(function () {
                if (lookupWindow.closed) {
                    clearInterval(checkClosed);
                    refreshInsuranceCompanies();
                }
            }, 500);
        }

        // Function to refresh insurance companies dropdown
        function refreshInsuranceCompanies() {
            // Trigger postback to refresh the dropdown
            __doPostBack('refreshInsuranceCompanies', '');
        }


    </script>

</asp:Content>