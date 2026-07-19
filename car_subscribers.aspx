<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="car_subscribers.aspx.cs"
    Inherits="car_WebApplication1.car_subscribers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }

        .subscriber-box {
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
                .btn-search {
            background-color: #a50d25;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-back:hover {
            background-color: #5a6268;
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

        .subscriber-history {
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

        .current-subscriber {
            background-color: #e3f2fd;
            border: 2px solid #2196f3;
            border-radius: 10px;
            padding: 15px;
            margin-top: 20px;
        }

        .current-subscriber-label {
            font-size: 16px;
            color: #2196f3;
            font-weight: bold;
        }

        .current-subscriber-value {
            font-size: 20px;
            font-weight: bold;
        }

        .route-info {
            font-size: 14px;
            color: #6c757d;
            margin-top: 5px;
        }

        .cost-column {
            color: #28a745;
            font-weight: bold;
        }

        .btn-search {
            background-color: #a50d25;
            color: #fff;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-search:hover {
            background-color: #a50d25;
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
            <h2 class="page-title">بيانات المشتركين</h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" 
            CssClass="alert alert-danger text-center d-block" 
            Visible="false">
        </asp:Label>

        <asp:Label ID="lblSuccess" runat="server" 
            CssClass="alert alert-success text-center d-block" 
            Visible="false">
        </asp:Label>

        <div class="subscriber-box">

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



            <!-- نموذج المشتركين -->
            <asp:Panel ID="pnlSubscriberForm" runat="server" Visible="false">
                <div class="section-header">
                    <i class="fas fa-users"></i> إضافة اشتراك جديد
                </div>

                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label">المشترك</label>
                        <asp:DropDownList ID="ddlSubscriber" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر المشترك --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">المسار</label>
                        <asp:DropDownList ID="ddlRoute" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRoute_SelectedIndexChanged">
                            <asp:ListItem Value="">-- اختر المسار --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">تاريخ البداية</label>
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">تاريخ النهاية</label>
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">التكلفة</label>
                        <asp:TextBox ID="txtCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" placeholder="0.00"></asp:TextBox>
                    </div>
                    <div class="col-md-12">
                        <label class="form-label">ملاحظات</label>
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    </div>
                </div>

                <!-- Display route details when selected -->
                <asp:Panel ID="pnlRouteDetails" runat="server" Visible="false" CssClass="route-info mt-3">
                    <div class="alert alert-info">
                        <strong>تفاصيل المسار:</strong><br />
                        <asp:Label ID="lblRouteDetails" runat="server"></asp:Label>
                    </div>
                </asp:Panel>

                <div class="row mt-4">
                    <div class="col-md-6">
                        <asp:Button ID="btnSaveSubscriber" runat="server" 
                            Text="حفظ الاشتراك" 
                            CssClass="btn btn-save" 
                            OnClick="btnSaveSubscriber_Click" />
                    </div>
                    <div class="col-md-6">
                        <asp:Button ID="btnBack" runat="server" 
                            Text="رجوع" 
                            CssClass="btn btn-back" 
                            OnClick="btnBack_Click" />
                    </div>
                </div>
            </asp:Panel>

            <!-- سجل المشتركين السابقين -->
            <asp:Panel ID="pnlSubscriberHistory" runat="server" Visible="false" CssClass="subscriber-history">
                <div class="section-header">
                    <i class="fas fa-history"></i> سجل الاشتراكات السابقة
                </div>

                <asp:GridView ID="gvSubscriberHistory" runat="server" 
                    CssClass="grid-view" 
                    AutoGenerateColumns="False" 
                    EmptyDataText="لا توجد اشتراكات سابقة"
                    ShowHeaderWhenEmpty="True"
                    AllowPaging="True"
                    PageSize="5"
                    OnPageIndexChanging="gvSubscriberHistory_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="subscriber_name" HeaderText="اسم المشترك" />
                        <asp:BoundField DataField="route_name" HeaderText="اسم المسار" />
                        <asp:BoundField DataField="start_loc" HeaderText="من" />
                        <asp:BoundField DataField="end_loc" HeaderText="إلى" />
                        <asp:BoundField DataField="KM" HeaderText="المسافة (كم)" DataFormatString="{0:N0}" />
                        <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="cost" HeaderText="التكلفة" DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
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
    </script>

</asp:Content>