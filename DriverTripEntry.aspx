<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" 
    CodeBehind="DriverTripEntry.aspx.cs" Inherits="car_WebApplication1.DriverTripEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            font-size: 28px;
            margin-bottom: 25px;
        }
        
        .result-box {
            border: 2px solid #000;
            border-radius: 14px;
            padding: 25px;
            margin-top: 20px;
            background-color: #f9f9f9;
        }
        
        .form-label {
            font-weight: bold;
            margin-bottom: 8px;
            color: #333;
            font-size: 14px;
        }
        
        .form-control {
            text-align: right;
            height: 45px;
            font-size: 15px;
        }
        
        .enhanced-dropdown {
            background: #fff;
            border: 2px solid #dee2e6;
            border-radius: 8px;
            transition: all 0.3s ease;
            padding: 0 10px;
        }
        
        .enhanced-dropdown:focus {
            border-color: #c8102e;
            box-shadow: 0 0 0 3px rgba(200, 16, 46, 0.1);
            outline: none;
        }
        
        .section-header {
            background: linear-gradient(90deg, #c8102e, #ffffff);
            padding: 10px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-weight: bold;
            color: #000;
            font-size: 18px;
        }
        
        .section-header i {
            margin-left: 10px;
            color: #c8102e;
        }
        
        .btn-save {
            background: #28a745;
            color: white;
            border: none;
            height: 50px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        
        .btn-save:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .btn-clear {
            background: #6c757d;
            color: white;
            border: none;
            height: 50px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        
        .btn-clear:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .btn-cancel {
            background: #dc3545;
            color: white;
            border: none;
            height: 50px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        
        .btn-cancel:hover {
            background: #c82333;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .info-card {
            background: #e8f4f8;
            border-right: 4px solid #c8102e;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }
        
        .driver-info {
            font-size: 16px;
            margin-bottom: 10px;
        }
        
        .driver-info strong {
            color: #c8102e;
        }
        
        .readonly-field {
            background-color: #e9ecef;
        }
        
        .table-header {
            background-color: #c8102e;
            color: white;
        }
        
        .table-row:hover {
            background-color: #f5f5f5;
        }
        
        .table-alt-row {
            background-color: #f9f9f9;
        }
        
        .me-2 {
            margin-left: 8px;
        }
        
        .mt-2 {
            margin-top: 10px;
        }
        
        .mt-3 {
            margin-top: 15px;
        }
        
        .mb-4 {
            margin-bottom: 20px;
        }
        
        .w-100 {
            width: 100%;
        }
        
        .text-center {
            text-align: center;
        }
        
        .d-flex {
            display: flex;
        }
        
        .align-items-end {
            align-items: flex-end;
        }
        
        .alert {
            padding: 12px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
        }
        
        .alert-danger {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        
        .alert-success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        
        .alert-warning {
            background-color: #fff3cd;
            color: #856404;
            border: 1px solid #ffeeba;
        }
        
        .d-block {
            display: block;
        }
        
        .text-danger {
            color: #dc3545;
        }
        
        .text-muted {
            color: #6c757d;
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <div class="text-center">
            <h2 class="page-title">
                <i class="fas fa-truck-moving" style="color: #c8102e; margin-left: 10px;"></i>
                تسجيل بيانات الرحلة
            </h2>
        </div>

        <!-- رسائل -->
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblWarning" runat="server" CssClass="alert alert-warning text-center d-block" Visible="false"></asp:Label>

        <!-- البحث المتقدم -->
        <div class="result-box mb-4">
            <div class="section-header">
                <i class="fas fa-search"></i> بحث عن المأموريات
            </div>
            
            <div class="row g-3">
                <div class="col-md-4">
                    <label class="form-label">رقم المأمورية</label>
                    <asp:TextBox ID="txtMissionNumberSearch" runat="server" CssClass="form-control enhanced-dropdown" 
                        placeholder="أدخل رقم المأمورية" TextMode="Number"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label">اسم السائق <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtDriverNameSearch" runat="server" CssClass="form-control enhanced-dropdown" 
                        placeholder="أدخل اسم السائق"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDriverName" runat="server" 
                        ControlToValidate="txtDriverNameSearch" 
                        ErrorMessage="الرجاء إدخال اسم السائق"
                        CssClass="text-danger" Display="Dynamic" />
                </div>
                <div class="col-md-4 d-flex align-items-end">
                    <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-save w-100 me-2" 
                        OnClick="btnSearch_Click" ValidationGroup="Search" />
                    <asp:Button ID="btnReset" runat="server" Text="إعادة تعيين" CssClass="btn btn-clear w-100" 
                        OnClick="btnReset_Click" />
                </div>
            </div>
            
            <div class="row g-3 mt-2">
                <div class="col-md-12">
                    <small class="text-muted">ملاحظة: رقم المأمورية اختياري، يمكنك تركه فارغاً للبحث باسم السائق فقط</small>
                </div>
            </div>
        </div>

        <!-- قائمة المأموريات -->
        <div id="workOrdersPanel" runat="server" class="result-box mb-4" visible="false">
            <div class="section-header">
                <i class="fas fa-list"></i> المأموريات
            </div>
            
            <div class="table-responsive">
                <asp:GridView ID="gvWorkOrders" runat="server" 
                    CssClass="table table-striped table-bordered table-hover"
                    AutoGenerateColumns="false"
                    EmptyDataText="لا توجد مأموريات"
                    OnRowCommand="gvWorkOrders_RowCommand"
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="mission_id" HeaderText="رقم المأمورية" />
                        <asp:BoundField DataField="driver_name" HeaderText="اسم السائق" />
                        <asp:BoundField DataField="car_number" HeaderText="رقم السيارة" />
                        <asp:BoundField DataField="mission_type" HeaderText="نوع المأمورية" />
                        <asp:TemplateField HeaderText="إجراءات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnSelect" runat="server" 
                                    CommandName="SelectMission" 
                                    CommandArgument='<%# Eval("mission_id") %>'
                                    CssClass="btn btn-success btn-sm">
                                    <i class="fas fa-edit"></i> إدخال البيانات
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="table-header" BackColor="#c8102e" ForeColor="White" />
                    <RowStyle CssClass="table-row" />
                    <AlternatingRowStyle CssClass="table-alt-row" />
                    <PagerStyle CssClass="grid-pager" />
                </asp:GridView>
            </div>
        </div>

        <!-- معلومات المأمورية -->
        <div id="missionInfo" runat="server" class="result-box mb-4" visible="false">
            <div class="section-header">
                <i class="fas fa-info-circle"></i> معلومات المأمورية
            </div>
            
            <div class="info-card">
                <div class="driver-info">
                    <strong>تاريخ أمر الشغل:</strong> <asp:Label ID="lblWorkOrderDate" runat="server" Text=""></asp:Label>
                </div>
                <div class="driver-info">
                    <strong>رقم السيارة:</strong> <asp:TextBox ID="txtCarNumber" runat="server" CssClass="form-control enhanced-dropdown" 
                        ReadOnly="true" BackColor="#e9ecef" style="display: inline-block; width: auto;"></asp:TextBox>
                </div>
                <div class="driver-info">
                    <strong>السائق:</strong> <asp:Label ID="lblDriverName" runat="server" Text="" style="font-weight: normal;"></asp:Label>
                </div>
                <div class="driver-info">
                    <strong>نوع المأمورية:</strong> <asp:Label ID="lblMissionType" runat="server" Text="" style="font-weight: normal;"></asp:Label>
                </div>
                <div class="driver-info">
                    <strong>التفاصيل:</strong> <asp:Label ID="lblMissionDetails" runat="server" Text="" style="font-weight: normal;"></asp:Label>
                </div>
            </div>
            
            <!-- بيانات الوقت والمسافة -->
            <div class="section-header mt-3">
                <i class="fas fa-clock"></i> بيانات الوقت والمسافة
            </div>
            
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">بداية المأمورية <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control enhanced-dropdown" 
                        TextMode="DateTimeLocal"></asp:TextBox>
                </div>
                <div class="col-md-6">
                    <label class="form-label">وقت العودة <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtReturnTime" runat="server" CssClass="form-control enhanced-dropdown" 
                        TextMode="DateTimeLocal"></asp:TextBox>
                </div>
            </div>
            
            <div class="row g-3 mt-2">
                <div class="col-md-6">
                    <label class="form-label">عداد البداية (كم)</label>
                    <asp:TextBox ID="txtStartKM" runat="server" CssClass="form-control enhanced-dropdown readonly-field" 
                        placeholder="عداد البداية" TextMode="Number" step="0.1" min="0" ReadOnly="true" BackColor="#F8F9FA"></asp:TextBox>
                </div>
                <div class="col-md-6">
                    <label class="form-label">عداد العودة (كم) <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtEndKM" runat="server" CssClass="form-control enhanced-dropdown" 
                        placeholder="أدخل عداد العودة" TextMode="Number" step="0.1" min="0"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEndKM" runat="server" 
                        ControlToValidate="txtEndKM" 
                        ErrorMessage="الرجاء إدخال عداد العودة"
                        CssClass="text-danger" Display="Dynamic" />
                </div>
            </div>
            
            <div class="row g-3 mt-4">
                <div class="col-md-6">
                    <asp:Button ID="btnSave" runat="server" Text="حفظ البيانات" CssClass="btn btn-save w-100" 
                        OnClick="btnSave_Click" OnClientClick="return validateForm();" />
                </div>
                <div class="col-md-6">
                    <asp:Button ID="btnCancel" runat="server" Text="إلغاء" CssClass="btn btn-cancel w-100" 
                        OnClick="btnCancel_Click" />
                </div>
            </div>
        </div>

    </div>

    <script type="text/javascript">
        function validateForm() {
            var endKM = document.getElementById('<%= txtEndKM.ClientID %>').value;
            var startTime = document.getElementById('<%= txtStartTime.ClientID %>').value;
            var returnTime = document.getElementById('<%= txtReturnTime.ClientID %>').value;

            if (!startTime) {
                alert('الرجاء إدخال بداية المأمورية');
                return false;
            }

            if (!returnTime) {
                alert('الرجاء إدخال وقت العودة');
                return false;
            }

            if (!endKM) {
                alert('الرجاء إدخال عداد العودة');
                return false;
            }

            var start = new Date(startTime);
            var end = new Date(returnTime);

            if (end <= start) {
                alert('وقت العودة يجب أن يكون بعد وقت البداية');
                return false;
            }

            return true;
        }
    </script>

</asp:Content>