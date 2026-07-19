<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="vehicle_license.aspx.cs"
    Inherits="car_WebApplication1.vehicle_license" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }

        .license-box {
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
            overflow: auto;
        }

        .form-label {
            font-weight: bold;
        }

        .required-field::after {
            content: " *";
            color: #ce1126;
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

        .btn-renew {
            background-color: #28a745;
            border-color: #28a745;
            color: white;
            font-weight: bold;
            height: 45px;
        }

        .btn-renew:hover {
            background-color: #218838;
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

        .car-info-card {
            background: #808080;
            color: white;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 20px;
        }

        .car-number-display {
            font-size: 28px;
            font-weight: bold;
            text-align: center;
            letter-spacing: 2px;
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

        .current-license-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }

        .current-license-label {
            font-size: 13px;
            opacity: 0.9;
            margin-bottom: 5px;
        }

        .current-license-value {
            font-size: 18px;
            font-weight: bold;
        }

        .expiry-warning {
            background-color: #fff3cd;
            border-right: 4px solid #ffc107;
            padding: 10px;
            margin-top: 10px;
            border-radius: 4px;
            color: #856404;
        }

        .expiry-danger {
            background-color: #f8d7da;
            border-right: 4px solid #dc3545;
            color: #721c24;
        }

        .expiry-good {
            background-color: #d4edda;
            border-right: 4px solid #28a745;
            color: #155724;
        }

        .license-history {
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

        .status-warning {
            color: #ffc107;
            font-weight: bold;
        }

        .pagination-ys {
            background: #f8f9fa;
            padding: 10px;
            text-align: center;
            border-radius: 0 0 10px 10px;
        }

        .pagination-ys a, .pagination-ys span {
            display: inline-block;
            padding: 5px 10px;
            margin: 0 2px;
            border: 1px solid #dee2e6;
            border-radius: 4px;
            text-decoration: none;
            color: #a50d25;
            font-weight: bold;
        }

        .pagination-ys a:hover {
            background-color: #a50d25;
            color: white;
        }

        .pagination-ys span {
            background-color: #a50d25;
            color: white;
        }

        .modal-header {
            background-color: #a50d25;
            color: white;
        }

        .modal-header .close {
            color: white;
            opacity: 1;
        }

        .float-left {
            float: left !important;
        }

        .btn-sm {
            padding: 5px 10px;
            font-size: 12px;
            margin: 0 2px;
        }

        .btn-info {
            background-color: #17a2b8;
            border-color: #17a2b8;
            color: white;
        }

        .btn-danger {
            background-color: #dc3545;
            border-color: #dc3545;
            color: white;
        }

        .alert-warning-custom {
            background-color: #fff3cd;
            border-color: #ffc107;
            color: #856404;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            text-align: center;
        }
        
        .insurance-info-card {
            background: #e8f4f8;
            border-right: 4px solid #17a2b8;
            padding: 15px;
            margin-top: 15px;
            border-radius: 8px;
        }
        
        .insurance-label {
            font-size: 12px;
            color: #666;
            margin-bottom: 3px;
        }
        
        .insurance-value {
            font-size: 14px;
            font-weight: bold;
            color: #333;
        }

        @media (max-width: 768px) {
            .car-letter-box {
                width: 45px;
            }
            .car-number-single-box {
                width: 75px;
            }
            .car-number-display {
                font-size: 20px;
            }
        }
        /* Custom Modal Styles */
#viewLicenseModal {
    display: none;
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0,0,0,0.5);
    z-index: 9999;
    direction: rtl;
    overflow: auto;
}

#viewLicenseModal .close {
    background: transparent;
    border: none;
    color: white;
    font-size: 28px;
    cursor: pointer;
    padding: 0;
    line-height: 1;
}

#viewLicenseModal .btn-secondary {
    background-color: #6c757d;
    color: white;
    border: none;
    padding: 8px 20px;
    border-radius: 5px;
    cursor: pointer;
}

#viewLicenseModal .btn-secondary:hover {
    background-color: #5a6268;
}
#viewLicenseModal {
    display: none;
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0,0,0,0.7);
    z-index: 999999;
    direction: rtl;
}

#viewLicenseModal button {
    cursor: pointer;
}

#viewLicenseModal button:hover {
    opacity: 0.8;
}
    </style>

    <div class="container mt-4" dir="rtl">
        <div class="text-center mb-4">
            <h2 class="page-title">إدارة الترخيص</h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

        <div class="license-box">
            <!-- Search Section -->
            <div class="section-header">
                <i class="fas fa-search"></i> بحث برقم السيارة
            </div>

            <div class="row mb-4">
                <div class="col-md-8">
                    <label class="form-label">رقم السيارة</label>
                    <div class="car-number-container mb-2">
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter1" runat="server" CssClass="form-control car-letter" 
                                MaxLength="1" placeholder="أ" 
                                onkeyup="moveToNext(this, 'txtCarLetter2');" 
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, ''); }" 
                                onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter2" runat="server" CssClass="form-control car-letter" 
                                MaxLength="1" placeholder="ب" 
                                onkeyup="moveToNext(this, 'txtCarLetter3');" 
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter1'); }" 
                                onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="car-letter-box">
                            <asp:TextBox ID="txtCarLetter3" runat="server" CssClass="form-control car-letter" 
                                MaxLength="1" placeholder="ج" 
                                onkeyup="moveToNext(this, 'txtCarNumbers');" 
                                onkeydown="if(event.keyCode === 8 && this.value.length === 0) { moveToPrevious(this, 'txtCarLetter2'); }" 
                                onkeypress="return isArabicLetter(event);" autocomplete="off"></asp:TextBox>
                        </div>
                        <span class="car-number-separator">-</span>
                        <div class="car-number-single-box">
                            <asp:TextBox ID="txtCarNumbers" runat="server" CssClass="form-control car-number-single" 
                                MaxLength="4" placeholder="1234" 
                                onkeypress="return isNumber(event);" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <label class="form-label">&nbsp;</label>
                    <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn btn-search w-100" 
                        OnClick="BtnSearch_Click" OnClientClick="return validateCarNumber();" />
                </div>
            </div>

            <!-- Vehicle Info Panel -->
            <asp:Panel ID="pnlVehicleInfo" runat="server" Visible="false">
                <div class="car-info-card">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="car-number-display">
                                رقم السيارة: <asp:Label ID="lblCarNumber" runat="server" Font-Bold="true"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Current License Panel -->
            <asp:Panel ID="pnlCurrentLicense" runat="server" Visible="false">
                <div class="section-header">
                    <i class="fas fa-id-card"></i> الترخيص الحالي
                </div>
                <div class="current-license-card">
                    <div class="row">
                        <div class="col-md-3">
                            <div class="current-license-label">رقم الترخيص</div>
                            <div class="current-license-value"><asp:Label ID="lblCurrentLicenseNumber" runat="server"></asp:Label></div>
                        </div>
                        <div class="col-md-3">
                            <div class="current-license-label">نوع الترخيص</div>
                            <div class="current-license-value"><asp:Label ID="lblCurrentLicenseType" runat="server"></asp:Label></div>
                        </div>
                        <div class="col-md-3">
                            <div class="current-license-label">تاريخ الإصدار</div>
                            <div class="current-license-value"><asp:Label ID="lblCurrentIssueDate" runat="server"></asp:Label></div>
                        </div>
                        <div class="col-md-3">
                            <div class="current-license-label">تاريخ الانتهاء</div>
                            <div class="current-license-value"><asp:Label ID="lblCurrentExpiryDate" runat="server"></asp:Label></div>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4">
                            <div class="current-license-label">وحدة المرور</div>
                            <div class="current-license-value"><asp:Label ID="lblCurrentTrafficUnit" runat="server"></asp:Label></div>
                        </div>
                        <div class="col-md-4">
                            <div class="current-license-label">تكلفة التجديد</div>
                            <div class="current-license-value"><asp:Label ID="lblCurrentRenewCost" runat="server"></asp:Label></div>
                        </div>
                        <div class="col-md-4">
                            <div class="current-license-label">رقم التسجيل</div>
                            <div class="current-license-value"><asp:Label ID="lblCurrentRegistrationNumber" runat="server"></asp:Label></div>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-12">
                            <div class="current-license-label">ملاحظات</div>
                            <div class="current-license-value"><asp:Label ID="lblCurrentNotes" runat="server"></asp:Label></div>
                        </div>
                    </div>
                </div>
                
                <!-- Mandatory Insurance Info -->
                <asp:Panel ID="pnlMandatoryInsuranceInfo" runat="server" Visible="false" CssClass="insurance-info-card">
                    <div class="row">
                        <div class="col-md-12">
                            <strong><i class="fas fa-shield-alt"></i> التأمين الإجباري المرتبط بالترخيص</strong>
                        </div>
                        <div class="col-md-4 mt-2">
                            <div class="insurance-label">شركة التأمين</div>
                            <div class="insurance-value"><asp:Label ID="lblMandatoryInsuranceCompany" runat="server"></asp:Label></div>
                        </div>
                        <div class="col-md-4 mt-2">
                            <div class="insurance-label">التكلفة</div>
                            <div class="insurance-value"><asp:Label ID="lblMandatoryInsuranceCost" runat="server"></asp:Label></div>
                        </div>
                        <div class="col-md-4 mt-2">
                            <div class="insurance-label">ملاحظات</div>
                            <div class="insurance-value"><asp:Label ID="lblMandatoryInsuranceNotes" runat="server"></asp:Label></div>
                        </div>
                    </div>
                </asp:Panel>
                
                <!-- Warning message if license is still active -->
                <asp:Panel ID="pnlActiveLicenseWarning" runat="server" Visible="false" CssClass="alert-warning-custom">
                    <i class="fas fa-exclamation-triangle"></i>
                    <asp:Label ID="lblActiveLicenseWarning" runat="server"></asp:Label>
                </asp:Panel>
            </asp:Panel>

            <!-- Add/Renew License Form -->
            <asp:Panel ID="pnlLicenseForm" runat="server" Visible="false">
                <div class="section-header">
                    <i class="fas fa-plus-circle"></i> 
                    <asp:Label ID="lblFormTitle" runat="server" Text="إضافة ترخيص جديد"></asp:Label>
                </div>

                <div class="row g-3">
                    <div class="col-md-4">
                        <label class="form-label required-field">رقم الترخيص</label>
                        <asp:TextBox ID="txtLicenseNumber" runat="server" CssClass="form-control" MaxLength="50" placeholder="مثال: 123456789"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">نوع الترخيص</label>
                        <asp:DropDownList ID="ddlLicenseType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر نوع الترخيص --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">تاريخ الإصدار</label>
                        <asp:TextBox ID="txtIssueDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">تاريخ الانتهاء</label>
                        <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">وحدة المرور</label>
                        <asp:DropDownList ID="ddlTrafficUnit" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر وحدة المرور --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">تكلفة التجديد</label>
                        <asp:TextBox ID="txtRenewCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">رقم التسجيل</label>
                        <asp:TextBox ID="txtRegistrationNumber" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">تاريخ الفحص الفني</label>
                        <asp:TextBox ID="txtTechInspectionDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">انتهاء الفحص الفني</label>
                        <asp:TextBox ID="txtTechInspectionExpiry" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-12">
                        <label class="form-label">ملاحظات الترخيص</label>
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="ملاحظات إضافية عن الترخيص"></asp:TextBox>
                    </div>
                    
                    <!-- Mandatory Insurance Section -->
                    <div class="col-md-12 mt-3">
                        <hr />
                        <h5 class="text-primary"><i class="fas fa-shield-alt"></i> بيانات التأمين الإجباري</h5>
                        <small class="text-muted">التأمين الإجباري يرتبط بالترخيص ويبدأ وينتهي معه</small>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label required-field">شركة التأمين الإجباري</label>
                        <asp:DropDownList ID="ddlMandatoryInsurance" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- اختر شركة التأمين --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">تكلفة التأمين الإجباري</label>
                        <asp:TextBox ID="txtMandatoryInsuranceCost" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">ملاحظات التأمين</label>
                        <asp:TextBox ID="txtMandatoryInsuranceNotes" runat="server" CssClass="form-control" placeholder="ملاحظات عن التأمين الإجباري"></asp:TextBox>
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-md-6">
                        <asp:Button ID="btnSaveLicense" runat="server" Text="حفظ الترخيص والتأمين" CssClass="btn btn-save" 
                            OnClick="BtnSaveLicense_Click" OnClientClick="return validateLicenseForm();" />
                    </div>
                    <div class="col-md-6">
                        <asp:Button ID="btnCancelForm" runat="server" Text="إلغاء" CssClass="btn btn-back" 
                            OnClick="BtnCancelForm_Click" CausesValidation="false" />
                    </div>
                </div>
            </asp:Panel>

            <!-- License History Grid -->
            <asp:Panel ID="pnlLicenseHistory" runat="server" Visible="false" CssClass="license-history">
                <div class="section-header">
                    <i class="fas fa-history"></i> سجل التراخيص السابقة
                    <asp:Button ID="btnNewLicense" runat="server" Text="+ ترخيص جديد" CssClass="btn btn-sm btn-renew float-left" 
                        OnClick="BtnNewLicense_Click" CausesValidation="false" 
                        Visible="false" />
                </div>

<asp:GridView ID="gvLicenseHistory" runat="server" CssClass="grid-view" 
    AutoGenerateColumns="false" EmptyDataText="لا توجد سجلات تراخيص سابقة"
    AllowPaging="true" PageSize="10" OnPageIndexChanging="GvLicenseHistory_PageIndexChanging"
    OnRowDataBound="GvLicenseHistory_RowDataBound">
    <Columns>
        <asp:BoundField DataField="license_number" HeaderText="رقم الترخيص" />
        <asp:BoundField DataField="license_type" HeaderText="نوع الترخيص" />
        <asp:BoundField DataField="issue_date" HeaderText="تاريخ الإصدار" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="expiry_date" HeaderText="تاريخ الانتهاء" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="traffic_unit" HeaderText="وحدة المرور" />
        <asp:BoundField DataField="renewal_cost" HeaderText="تكلفة التجديد" DataFormatString="{0:N2}" />
        <asp:BoundField DataField="mandatory_insurance" HeaderText="التأمين الإجباري" />
        <asp:TemplateField HeaderText="الحالة">
            <ItemTemplate>
                <asp:Label ID="lblStatus" runat="server" 
                    CssClass='<%# GetStatusCss(Convert.ToDateTime(Eval("expiry_date"))) %>'
                    Text='<%# GetStatusText(Convert.ToDateTime(Eval("expiry_date"))) %>'>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="إجراءات">
            <ItemTemplate>
                <asp:LinkButton ID="lnkView" runat="server" CssClass="btn btn-sm btn-info" 
                    OnClick="ViewLicense_Click" CommandArgument='<%# Eval("license_id") %>'
                    ToolTip="عرض التفاصيل" CausesValidation="false">
                    <i class="fas fa-eye"></i> عرض
                </asp:LinkButton>
                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-danger" 
                    OnClick="DeleteLicense_Click" CommandArgument='<%# Eval("license_id") %>'
                    ToolTip="حذف" OnClientClick="return confirm('هل أنت متأكد من حذف هذا الترخيص؟');" 
                    CausesValidation="false">
                    <i class="fas fa-trash"></i> حذف
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
</asp:GridView>
            </asp:Panel>
        </div>
    </div>
<!-- Simple Custom Modal -->
<div id="viewLicenseModal" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.6); z-index: 10000; direction: rtl;">
    <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); width: 85%; max-width: 800px; background: white; border-radius: 12px; box-shadow: 0 5px 25px rgba(0,0,0,0.3);">
        <div style="background: #a50d25; color: white; padding: 15px 20px; border-radius: 12px 12px 0 0; display: flex; justify-content: space-between; align-items: center;">
            <h5 style="margin: 0;"><i class="fas fa-id-card"></i> <span id="licenseModalTitle">تفاصيل الترخيص</span></h5>
            <button onclick="closeLicenseModal()" style="background: none; border: none; color: white; font-size: 28px; cursor: pointer;">&times;</button>
        </div>
        <div style="padding: 20px; max-height: 450px; overflow-y: auto;">
            <div id="licenseDetailsContainer"></div>
        </div>
        <div style="padding: 15px 20px; border-top: 1px solid #ddd; text-align: left;">
            <button onclick="closeLicenseModal()" style="background: #6c757d; color: white; border: none; padding: 8px 20px; border-radius: 5px; cursor: pointer;">إغلاق</button>
        </div>
    </div>
</div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
 

    <script type="text/javascript">
        // Car number functions
        function moveToNext(current, nextFieldId) {
            if (current.value.length === current.maxLength) {
                var nextField = document.getElementById(nextFieldId);
                if (nextField) nextField.focus();
            }
        }

        function moveToPrevious(current, prevFieldId) {
            if (current.value.length === 0 && prevFieldId) {
                var prevField = document.getElementById(prevFieldId);
                if (prevField) prevField.focus();
            }
        }

        function isArabicLetter(event) {
            var charCode = event.which || event.keyCode;
            var charStr = String.fromCharCode(charCode);
            var arabicPattern = /[\u0600-\u06FF]/;
            if (charCode === 8 || charCode === 9 || charCode === 13 || charCode === 37 || charCode === 39 || charCode === 46) return true;
            return arabicPattern.test(charStr);
        }

        function isNumber(event) {
            var charCode = event.which || event.keyCode;
            if (charCode >= 48 && charCode <= 57) return true;
            if (charCode === 8 || charCode === 9 || charCode === 13 || charCode === 37 || charCode === 39 || charCode === 46) return true;
            var arabicNumberPattern = /[\u0660-\u0669]/;
            return arabicNumberPattern.test(String.fromCharCode(charCode));
        }

        function validateCarNumber() {
            var letter1 = document.getElementById('<%= txtCarLetter1.ClientID %>');
            var numbers = document.getElementById('<%= txtCarNumbers.ClientID %>');
            if (!letter1.value || !numbers.value) {
                alert('❌ الرجاء إدخال رقم السيارة (حرف واحد على الأقل والأرقام)');
                return false;
            }
            return true;
        }

        function validateLicenseForm() {
            var licenseNumber = document.getElementById('<%= txtLicenseNumber.ClientID %>');
            var licenseType = document.getElementById('<%= ddlLicenseType.ClientID %>');
            var issueDate = document.getElementById('<%= txtIssueDate.ClientID %>');
            var expiryDate = document.getElementById('<%= txtExpiryDate.ClientID %>');

            if (!licenseNumber.value) {
                alert('❌ الرجاء إدخال رقم الترخيص');
                licenseNumber.focus();
                return false;
            }
            if (licenseType.value === '') {
                alert('❌ الرجاء اختيار نوع الترخيص');
                licenseType.focus();
                return false;
            }
            if (!issueDate.value) {
                alert('❌ الرجاء إدخال تاريخ الإصدار');
                issueDate.focus();
                return false;
            }
            if (!expiryDate.value) {
                alert('❌ الرجاء إدخال تاريخ الانتهاء');
                expiryDate.focus();
                return false;
            }

            var issue = new Date(issueDate.value);
            var expiry = new Date(expiryDate.value);

            if (expiry <= issue) {
                alert('❌ تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار');
                expiryDate.focus();
                return false;
            }

            return true;
        }

        function showLicenseDetails(data) {
            var container = document.getElementById('licenseDetailsContainer');
            if (!container) return;

            var html = `
                <div class="col-md-6 mb-2"><strong>رقم الترخيص:</strong> ${data.license_number || '-'}</div>
                <div class="col-md-6 mb-2"><strong>نوع الترخيص:</strong> ${data.license_type || '-'}</div>
                <div class="col-md-6 mb-2"><strong>تاريخ الإصدار:</strong> ${data.issue_date || '-'}</div>
                <div class="col-md-6 mb-2"><strong>تاريخ الانتهاء:</strong> ${data.expiry_date || '-'}</div>
                <div class="col-md-6 mb-2"><strong>وحدة المرور:</strong> ${data.traffic_unit || '-'}</div>
                <div class="col-md-6 mb-2"><strong>تكلفة التجديد:</strong> ${data.renewal_cost || '0'} ج.م</div>
                <div class="col-md-6 mb-2"><strong>رقم التسجيل:</strong> ${data.registration_number || '-'}</div>
                <div class="col-md-6 mb-2"><strong>تاريخ الفحص الفني:</strong> ${data.tech_inspection_date || '-'}</div>
                <div class="col-md-6 mb-2"><strong>انتهاء الفحص الفني:</strong> ${data.tech_inspection_expiry || '-'}</div>
                <div class="col-md-12 mb-2"><strong>ملاحظات الترخيص:</strong> ${data.notes || '-'}</div>`;
            
            if (data.mandatory_insurance_company) {
                html += `<div class="col-md-12 mt-3"><hr><h6 class="text-primary">التأمين الإجباري المرتبط</h6></div>`;
                html += `<div class="col-md-6 mb-2"><strong>شركة التأمين:</strong> ${data.mandatory_insurance_company || '-'}</div>`;
                html += `<div class="col-md-6 mb-2"><strong>تكلفة التأمين:</strong> ${data.mandatory_insurance_cost || '0'} ج.م</div>`;
                html += `<div class="col-md-12 mb-2"><strong>ملاحظات التأمين:</strong> ${data.mandatory_insurance_notes || '-'}</div>`;
            }
            
            container.innerHTML = html;
        }

        // Set default dates on page load
        document.addEventListener('DOMContentLoaded', function() {
            var today = new Date().toISOString().split('T')[0];
            var nextYear = new Date();
            nextYear.setFullYear(nextYear.getFullYear() + 1);
            var nextYearStr = nextYear.toISOString().split('T')[0];

            var issueDate = document.getElementById('<%= txtIssueDate.ClientID %>');
            var expiryDate = document.getElementById('<%= txtExpiryDate.ClientID %>');

            if (issueDate && !issueDate.value) {
                issueDate.value = today;
            }
            if (expiryDate && !expiryDate.value) {
                expiryDate.value = nextYearStr;
            }
        });

        // Global variable to track modal state
        // Global flag to track modal state
        var isModalOpen = false;

        // Function to show license details
        function showLicenseDetailsWithInsurance(data) {
            try {
                var container = document.getElementById('licenseDetailsContainer');
                if (!container) return;

                var html = '<div style="display: flex; flex-wrap: wrap;">';

                // License details
                html += '<div style="width: 50%; padding: 8px;"><strong>رقم الترخيص:</strong><br><span>' + (data.license_number || '-') + '</span></div>';
                html += '<div style="width: 50%; padding: 8px;"><strong>نوع الترخيص:</strong><br><span>' + (data.license_type || '-') + '</span></div>';
                html += '<div style="width: 50%; padding: 8px;"><strong>تاريخ الإصدار:</strong><br><span>' + (data.issue_date || '-') + '</span></div>';
                html += '<div style="width: 50%; padding: 8px;"><strong>تاريخ الانتهاء:</strong><br><span>' + (data.expiry_date || '-') + '</span></div>';
                html += '<div style="width: 50%; padding: 8px;"><strong>وحدة المرور:</strong><br><span>' + (data.traffic_unit || '-') + '</span></div>';
                html += '<div style="width: 50%; padding: 8px;"><strong>تكلفة التجديد:</strong><br><span>' + (data.renewal_cost || '0') + ' ج.م</span></div>';
                html += '<div style="width: 50%; padding: 8px;"><strong>رقم التسجيل:</strong><br><span>' + (data.registration_number || '-') + '</span></div>';
                html += '<div style="width: 50%; padding: 8px;"><strong>تاريخ الفحص الفني:</strong><br><span>' + (data.tech_inspection_date || '-') + '</span></div>';
                html += '<div style="width: 50%; padding: 8px;"><strong>انتهاء الفحص الفني:</strong><br><span>' + (data.tech_inspection_expiry || '-') + '</span></div>';
                html += '<div style="width: 100%; padding: 8px;"><strong>ملاحظات الترخيص:</strong><br><span>' + (data.notes || '-') + '</span></div>';

                // Mandatory insurance
                if (data.mandatory_insurance_company && data.mandatory_insurance_company !== '-') {
                    html += '<div style="width: 100%; margin-top: 15px; padding: 8px;"><hr><h5 style="color: #a50d25;">التأمين الإجباري المرتبط</h5></div>';
                    html += '<div style="width: 50%; padding: 8px;"><strong>شركة التأمين:</strong><br><span>' + (data.mandatory_insurance_company || '-') + '</span></div>';
                    html += '<div style="width: 50%; padding: 8px;"><strong>تكلفة التأمين:</strong><br><span>' + (data.mandatory_insurance_cost || '0') + ' ج.م</span></div>';
                    if (data.mandatory_insurance_start_date && data.mandatory_insurance_start_date !== '-') {
                        html += '<div style="width: 50%; padding: 8px;"><strong>بداية التأمين:</strong><br><span>' + data.mandatory_insurance_start_date + '</span></div>';
                        html += '<div style="width: 50%; padding: 8px;"><strong>نهاية التأمين:</strong><br><span>' + data.mandatory_insurance_end_date + '</span></div>';
                    }
                    html += '<div style="width: 100%; padding: 8px;"><strong>ملاحظات التأمين:</strong><br><span>' + (data.mandatory_insurance_notes || '-') + '</span></div>';
                }

                html += '</div>';
                container.innerHTML = html;

                // Show modal
                var modal = document.getElementById('viewLicenseModal');
                if (modal) {
                    modal.style.display = 'block';
                    isModalOpen = true;
                }

            } catch (e) {
                console.error('Error:', e);
                alert('حدث خطأ في عرض البيانات: ' + e.message);
            }
        }

        // Function to close modal
        function closeLicenseModal() {
            var modal = document.getElementById('viewLicenseModal');
            if (modal) {
                modal.style.display = 'none';
                isModalOpen = false;
            }
        }

        // Close modal when pressing ESC
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && isModalOpen) {
                closeLicenseModal();
            }
        });

        // Close modal when clicking on the overlay (but NOT on the modal content)
        document.addEventListener('click', function (e) {
            var modal = document.getElementById('viewLicenseModal');
            if (modal && modal.style.display === 'block') {
                // Check if clicked element is the modal background
                if (e.target === modal) {
                    closeLicenseModal();
                }
            }
        });

    </script>

</asp:Content>