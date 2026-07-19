<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="main_vehicle_entry.aspx.cs"
    Inherits="car_WebApplication1.main_vehicle_entry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }

        .entry-box {
            border: 2px solid #000;
            border-radius: 14px;
            padding: 25px;
            background-color: #fff;
            margin-bottom: 20px;
        }

        .form-label {
            font-weight: bold;
        }

        .form-control {
            text-align: right;
        }

        .btn-save {
            background-color: #c8102e;
            border-color: #c8102e;
            color: white;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-save:hover {
            background-color: #a50d25;
        }

        .btn-clear {
            background-color: #6c757d;
            color: #fff;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-clear:hover {
            background-color: #5a6268;
        }

        /* Car Number Container Styles */
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

        .car-letter:focus, .car-number-single:focus {
            outline: none;
            box-shadow: 0 0 0 3px rgba(206, 17, 38, 0.25);
            border-color: #ce1126;
        }

        .car-letter.error, .car-number-single.error {
            border-color: #dc3545;
            background-color: #fff8f8;
        }

        .car-letter.valid, .car-number-single.valid {
            border-color: #28a745;
            background-color: #f0fff0;
        }

        /* Section Headers */
        .section-header {
            background: linear-gradient(90deg, #ce1126, #ffffff);
            padding: 10px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-weight: bold;
            color: #000;
        }

        .info-card {
            background: white;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .required-field::after {
            content: " *";
            color: #ce1126;
            font-weight: bold;
        }

        @media (max-width: 768px) {
            .car-letter-box {
                width: 50px;
            }

            .car-number-single-box {
                width: 80px;
            }

            .car-number-separator {
                font-size: 16px;
            }
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <!-- العنوان -->
        <div class="text-center mb-4">
            <h2 class="page-title">إدخال بيانات سيارة جديدة</h2>
        </div>

        <!-- رسالة خطأ -->
        <asp:Label ID="lblMessage"
            runat="server"
            CssClass="alert alert-danger text-center d-block"
            Visible="false">
        </asp:Label>

        <!-- رسالة نجاح -->
        <asp:Label ID="lblSuccess"
            runat="server"
            CssClass="alert alert-success text-center d-block"
            Visible="false">
        </asp:Label>

        <div class="entry-box">

            <!-- ========== القسم 1: رقم السيارة ========== -->
            <div class="section-header">
                <i class="fas fa-car"></i> رقم السيارة <span class="required-field"></span>
            </div>

            <div class="info-card">
                <div class="row align-items-end">
                    <div class="col-md-12">
                        <label class="form-label required-field">رقم السيارة</label>
                        <!-- Car Number Component -->
                        <div class="car-number-container mb-2">
                            <!-- Letter 1 Box -->
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
                            
                            <!-- Letter 2 Box -->
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
                            
                            <!-- Letter 3 Box (Optional) -->
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
                            
                            <!-- Single Box for 1-4 digits -->
                            <div class="car-number-single-box">
                                <asp:TextBox ID="txtCarNumbers" runat="server"
                                    CssClass="form-control car-number-single"
                                    MaxLength="4"
                                    placeholder="1234"
                                    onkeypress="return isNumber(event);"
                                    autocomplete="off"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Validation Message -->
                        <div>
                            <small class="text-muted">الصيغة: حرف + حرف + حرف (اختياري) + ١-٤ أرقام</small>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ========== القسم 2: البيانات الأساسية ========== -->
            <div class="section-header">
                <i class="fas fa-info-circle"></i> البيانات الأساسية
            </div>

            <div class="info-card">
                <div class="row g-3">
                    <div class="col-md-4">
                        <label class="form-label required-field">نوع السيارة</label>
                        <asp:DropDownList ID="ddlCarType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر نوع السيارة --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">الموديل</label>
                        <asp:DropDownList ID="ddlCarModel" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر الموديل --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">سنة الصنع</label>
                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر السنة --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">حالة السيارة</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر الحالة --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">نوع الوقود</label>
                        <asp:DropDownList ID="ddlFuelType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر نوع الوقود --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">عدد الركاب</label>
                        <asp:TextBox ID="txtPassengers" runat="server" CssClass="form-control" TextMode="Number" min="1"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- ========== القسم 3: البيانات الفنية ========== -->
            <div class="section-header">
                <i class="fas fa-cogs"></i> البيانات الفنية
            </div>

            <div class="info-card">
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label required-field">رقم الشاسيه</label>
                        <asp:TextBox ID="txtChassis" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label required-field">رقم الموتور</label>
                        <asp:TextBox ID="txtMotor" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- ========== القسم 4: بيانات الترخيص ========== -->
            <div class="section-header">
                <i class="fas fa-id-card"></i> بيانات الترخيص
            </div>

            <div class="info-card">
                <div class="row g-3">
                    <div class="col-md-3">
                        <label class="form-label required-field">بداية الترخيص</label>
                        <asp:TextBox ID="txtLicenseStart" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label required-field">نهاية الترخيص</label>
                        <asp:TextBox ID="txtLicenseEnd" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label required-field">وحدة المرور</label>
                        <asp:DropDownList ID="ddlTrafficUnit" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر وحدة المرور --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">تكلفة التجديد</label>
                        <asp:TextBox ID="txtRenewCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">نوع الترخيص</label>
                        <asp:DropDownList ID="ddlLicenseType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر نوع الترخيص --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <!-- ========== القسم 5: قراءة العداد (الكيلومترات) ========== -->
            <div class="section-header">
                <i class="fas fa-tachometer-alt"></i> قراءة العداد
            </div>

            <div class="info-card">
                <div class="row g-3">
                    <div class="col-md-4">
                        <label class="form-label">قراءة العداد الحالية (كم)</label>
                        <asp:TextBox ID="txtCurrentOdometer" runat="server" 
                            CssClass="form-control" 
                            TextMode="Number" 
                            step="1" 
                            min="0"
                            placeholder="أدخل قراءة العداد"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">تاريخ القراءة</label>
                        <asp:TextBox ID="txtOdometerDate" runat="server" 
                            CssClass="form-control" 
                            TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">ملاحظات</label>
                        <asp:TextBox ID="txtOdometerNotes" runat="server" 
                            CssClass="form-control" 
                            placeholder="ملاحظات إضافية"></asp:TextBox>
                    </div>
                </div>
                <div class="row mt-2">
                    <div class="col-md-12">
                        <small class="text-muted">
                            <i class="fas fa-info-circle"></i> 
                            أدخل قراءة العداد الحالية للسيارة. سيتم استخدام هذه القراءة كنقطة بداية للرحلات المستقبلية.
                        </small>
                    </div>
                </div>
            </div>

            <!-- Buttons -->
            <div class="row mt-4">
                <div class="col-md-6">
                    <asp:Button ID="btnSave" runat="server" Text="حفظ البيانات" CssClass="btn btn-save" OnClick="btnSave_Click" OnClientClick="return validateForm();" />
                </div>
                <div class="col-md-6">
                    <asp:Button ID="btnClear" runat="server" Text="مسح الحقول" CssClass="btn btn-clear" OnClick="btnClear_Click" />
                </div>
            </div>

        </div>

    </div>

    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <!-- JavaScript -->
    <script type="text/javascript">
        // Move to next input field
        function moveToNext(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) {
                    nextField.focus();
                }
            }
            // Remove red color when user starts typing
            current.classList.remove('error');
        }

        // Move to previous field on backspace
        function moveToPrevious(current, prevFieldId) {
            if (current.value.length === 0 && prevFieldId) {
                var prevField = document.getElementById(prevFieldId);
                if (prevField) {
                    prevField.focus();
                }
            }
        }

        // Allow only Arabic letters
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

        // Allow only numbers
        function isNumber(event) {
            var charCode = event.which || event.keyCode;

            if (charCode >= 48 && charCode <= 57) return true;
            if (charCode === 8 || charCode === 9 || charCode === 13 ||
                charCode === 37 || charCode === 39 || charCode === 46) return true;

            var arabicNumberPattern = /[\u0660-\u0669]/;
            return arabicNumberPattern.test(String.fromCharCode(charCode));
        }

        // Validate car number
        function validateCarNumber(showAlert) {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtCarLetter2.ClientID %>');
            var letter3 = document.getElementById('<%= txtCarLetter3.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');

            if (!letter1 || !letter2 || !numbers) return false;

            // Remove all validation classes first
            letter1.classList.remove('error', 'valid');
            letter2.classList.remove('error', 'valid');
            if (letter3) letter3.classList.remove('error', 'valid');
            numbers.classList.remove('error', 'valid');

            var isValid = true;

            // Validate first letter (required)
            if (!letter1.value || letter1.value.trim() === '') {
                letter1.classList.add('error');
                isValid = false;
            } else {
                letter1.classList.add('valid');
            }

            // Validate second letter (required)
            if (!letter2.value || letter2.value.trim() === '') {
                letter2.classList.add('error');
                isValid = false;
            } else {
                letter2.classList.add('valid');
            }

            // Letter 3 is optional - just add valid class if it has value
            if (letter3 && letter3.value && letter3.value.trim() !== '') {
                letter3.classList.add('valid');
            } else if (letter3) {
                letter3.classList.remove('error', 'valid');
            }

            // Validate numbers - accept 1-4 digits
            if (!numbers.value || numbers.value.trim() === '') {
                numbers.classList.add('error');
                isValid = false;
            } else {
                // Check if it contains only numbers
                var numberStr = numbers.value.trim();
                var isValidNumber = true;

                for (var i = 0; i < numberStr.length; i++) {
                    var char = numberStr.charAt(i);
                    // Check if character is English number (0-9)
                    if (char >= '0' && char <= '9') continue;
                    // Check if character is Arabic number (٠-٩)
                    if (char >= '٠' && char <= '٩') continue;
                    // If not a number, invalid
                    isValidNumber = false;
                    break;
                }

                // Also check length (1-4 characters)
                if (numberStr.length < 1 || numberStr.length > 4) {
                    isValidNumber = false;
                }

                if (isValidNumber) {
                    numbers.classList.add('valid');
                } else {
                    numbers.classList.add('error');
                    isValid = false;
                }
            }

            if (!isValid && showAlert) {
                alert('الرجاء إدخال رقم سيارة صحيح (حرف + حرف + 1-4 أرقام)');
            }

            return isValid;
        }

        // Validate entire form
        function validateForm() {
            if (!validateCarNumber(true)) {
                return false;
            }

            var ddlCarType = document.getElementById('<%= ddlCarType.ClientID %>');
            if (!ddlCarType || ddlCarType.value === '') {
                alert('الرجاء اختيار نوع السيارة');
                ddlCarType.focus();
                return false;
            }

            var ddlCarModel = document.getElementById('<%= ddlCarModel.ClientID %>');
            if (!ddlCarModel || ddlCarModel.value === '') {
                alert('الرجاء اختيار الموديل');
                ddlCarModel.focus();
                return false;
            }

            var ddlYear = document.getElementById('<%= ddlYear.ClientID %>');
            if (!ddlYear || ddlYear.value === '') {
                alert('الرجاء اختيار سنة الصنع');
                ddlYear.focus();
                return false;
            }

            var ddlStatus = document.getElementById('<%= ddlStatus.ClientID %>');
            if (!ddlStatus || ddlStatus.value === '') {
                alert('الرجاء اختيار حالة السيارة');
                ddlStatus.focus();
                return false;
            }

            var ddlFuelType = document.getElementById('<%= ddlFuelType.ClientID %>');
            if (!ddlFuelType || ddlFuelType.value === '') {
                alert('الرجاء اختيار نوع الوقود');
                ddlFuelType.focus();
                return false;
            }

            var txtPassengers = document.getElementById('<%= txtPassengers.ClientID %>');
            if (!txtPassengers || txtPassengers.value === '') {
                alert('الرجاء إدخال عدد الركاب');
                txtPassengers.focus();
                return false;
            }

            var txtChassis = document.getElementById('<%= txtChassis.ClientID %>');
            if (!txtChassis || txtChassis.value === '') {
                alert('الرجاء إدخال رقم الشاسيه');
                txtChassis.focus();
                return false;
            }

            var txtMotor = document.getElementById('<%= txtMotor.ClientID %>');
            if (!txtMotor || txtMotor.value === '') {
                alert('الرجاء إدخال رقم الموتور');
                txtMotor.focus();
                return false;
            }

            var txtLicenseStart = document.getElementById('<%= txtLicenseStart.ClientID %>');
            if (!txtLicenseStart || txtLicenseStart.value === '') {
                alert('الرجاء إدخال تاريخ بداية الترخيص');
                txtLicenseStart.focus();
                return false;
            }

            var txtLicenseEnd = document.getElementById('<%= txtLicenseEnd.ClientID %>');
            if (!txtLicenseEnd || txtLicenseEnd.value === '') {
                alert('الرجاء إدخال تاريخ نهاية الترخيص');
                txtLicenseEnd.focus();
                return false;
            }

            if (new Date(txtLicenseEnd.value) <= new Date(txtLicenseStart.value)) {
                alert('تاريخ نهاية الترخيص يجب أن يكون بعد تاريخ البداية');
                return false;
            }

            var ddlTrafficUnit = document.getElementById('<%= ddlTrafficUnit.ClientID %>');
            if (!ddlTrafficUnit || ddlTrafficUnit.value === '') {
                alert('الرجاء اختيار وحدة المرور');
                ddlTrafficUnit.focus();
                return false;
            }

            var ddlLicenseType = document.getElementById('<%= ddlLicenseType.ClientID %>');
            if (!ddlLicenseType || ddlLicenseType.value === '') {
                alert('الرجاء اختيار نوع الترخيص');
                ddlLicenseType.focus();
                return false;
            }

            // Validate odometer reading if provided
            var txtCurrentOdometer = document.getElementById('<%= txtCurrentOdometer.ClientID %>');
            if (txtCurrentOdometer && txtCurrentOdometer.value !== '') {
                var odometerValue = parseFloat(txtCurrentOdometer.value);
                if (isNaN(odometerValue) || odometerValue < 0) {
                    alert('الرجاء إدخال قراءة عداد صحيحة (رقم موجب)');
                    txtCurrentOdometer.focus();
                    return false;
                }
            }

            return confirm('هل أنت متأكد من حفظ البيانات؟');
        }

        // Initialize on page load
        document.addEventListener('DOMContentLoaded', function() {
            // Add input event listeners to all car number fields
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var letter2 = document.getElementById('<%= txtCarLetter2.ClientID %>');
            var letter3 = document.getElementById('<%= txtCarLetter3.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');

            if (letter1) {
                letter1.addEventListener('input', function () { validateCarNumber(false); });
                letter1.addEventListener('blur', function () { validateCarNumber(false); });
            }
            if (letter2) {
                letter2.addEventListener('input', function () { validateCarNumber(false); });
                letter2.addEventListener('blur', function () { validateCarNumber(false); });
            }
            if (letter3) {
                letter3.addEventListener('input', function () { validateCarNumber(false); });
                letter3.addEventListener('blur', function () { validateCarNumber(false); });
            }
            if (numbers) {
                numbers.addEventListener('input', function () { validateCarNumber(false); });
                numbers.addEventListener('blur', function () { validateCarNumber(false); });
            }
        });
    </script>
</asp:Content>