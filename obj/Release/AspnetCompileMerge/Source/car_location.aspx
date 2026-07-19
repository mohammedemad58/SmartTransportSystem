<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="car_location.aspx.cs"
    Inherits="car_WebApplication1.car_location" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .page-title {
            font-weight: bold;
            color: #000;
            text-align: center;
            margin-bottom: 30px;
        }

        .routes-box {
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
            color: #fff;
            font-weight: bold;
            height: 50px;
            width: 100%;
        }

        .btn-search:hover {
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

        .btn-link-route {
            background-color: #28a745;
            color: white;
            border: none;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
            cursor: pointer;
            transition: all 0.3s;
            margin: 0 2px;
            display: inline-block;
        }

        .btn-link-route:hover {
            background-color: #218838;
        }

        .btn-edit-route {
            background-color: #ffc107;
            color: #212529;
            border: none;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
            cursor: pointer;
            transition: all 0.3s;
            margin: 0 2px;
            display: inline-block;
        }

        .btn-edit-route:hover {
            background-color: #e0a800;
        }

        .btn-delete-route {
            background-color: #dc3545;
            color: white;
            border: none;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
            cursor: pointer;
            transition: all 0.3s;
            margin: 0 2px;
            display: inline-block;
        }

        .btn-delete-route:hover {
            background-color: #c82333;
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

        .routes-list {
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

        .info-card {
            background: white;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .small-view {
            margin-top: 30px;
            border-top: 2px solid #dee2e6;
            padding-top: 20px;
        }

        .small-view-header {
            background-color: #f8f9fa;
            padding: 10px;
            border-radius: 8px;
            margin-bottom: 15px;
            font-weight: bold;
            color: #a50d25;
        }

        .route-item {
            background: #f8f9fa;
            border-right: 4px solid #a50d25;
            padding: 10px;
            margin-bottom: 10px;
            border-radius: 5px;
            transition: all 0.3s;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
        }

        .route-item:hover {
            background: #e9ecef;
            transform: translateX(-5px);
        }

        .route-info {
            flex: 1;
        }

        .route-name {
            font-weight: bold;
            color: #a50d25;
            font-size: 16px;
        }

        .route-details {
            font-size: 12px;
            color: #6c757d;
            margin-top: 5px;
        }

        .route-stats {
            margin-top: 5px;
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
        }

        .stat-badge {
            background-color: #e3f2fd;
            padding: 2px 8px;
            border-radius: 12px;
            font-size: 11px;
            color: #0d6efd;
        }

        .active-badge {
            background-color: #28a745;
            color: white;
            padding: 2px 8px;
            border-radius: 12px;
            font-size: 11px;
        }

        .inactive-badge {
            background-color: #dc3545;
            color: white;
            padding: 2px 8px;
            border-radius: 12px;
            font-size: 11px;
        }

        .route-actions {
            margin-right: 10px;
            white-space: nowrap;
        }

        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
        }

        .modal-content {
            background-color: #fefefe;
            margin: 5% auto;
            padding: 20px;
            border: 1px solid #888;
            width: 80%;
            max-width: 500px;
            border-radius: 10px;
            direction: rtl;
        }

        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-bottom: 1px solid #dee2e6;
            padding-bottom: 10px;
            margin-bottom: 20px;
        }

        .modal-footer {
            border-top: 1px solid #dee2e6;
            padding-top: 10px;
            margin-top: 20px;
            text-align: left;
        }

        .close {
            color: #aaa;
            float: left;
            font-size: 28px;
            font-weight: bold;
            cursor: pointer;
        }

        .close:hover {
            color: black;
        }

        @media (max-width: 768px) {
            .car-letter-box {
                width: 50px;
            }

            .car-number-single-box {
                width: 80px;
            }
            
            .route-item {
                flex-direction: column;
                align-items: flex-start;
            }
            
            .route-actions {
                margin-top: 10px;
                margin-right: 0;
            }
        }
    </style>

    <div class="container mt-4" dir="rtl">

        <div class="text-center mb-4">
            <h2 class="page-title">إدارة المسارات</h2>
        </div>

        <asp:Label ID="lblMessage" runat="server" 
            CssClass="alert alert-danger text-center d-block" 
            Visible="false">
        </asp:Label>

        <asp:Label ID="lblSuccess" runat="server" 
            CssClass="alert alert-success text-center d-block" 
            Visible="false">
        </asp:Label>

        <div class="routes-box">

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

            <!-- نموذج إضافة مسار -->
            <asp:Panel ID="pnlRouteForm" runat="server" Visible="false">
                <div class="section-header">
                    <i class="fas fa-plus-circle"></i> إضافة مسار جديد
                </div>

                <div class="info-card">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label">اسم المسار</label>
                            <asp:DropDownList ID="ddlRouteName" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">-- اختر اسم المسار --</asp:ListItem>
                                <asp:ListItem Value="القاهرة - الإسكندرية">القاهرة - الإسكندرية</asp:ListItem>
                                <asp:ListItem Value="القاهرة - الجيزة">القاهرة - الجيزة</asp:ListItem>
                                <asp:ListItem Value="القاهرة - شرم الشيخ">القاهرة - شرم الشيخ</asp:ListItem>
                                <asp:ListItem Value="القاهرة - الغردقة">القاهرة - الغردقة</asp:ListItem>
                                <asp:ListItem Value="الإسكندرية - مرسى مطروح">الإسكندرية - مرسى مطروح</asp:ListItem>
                                <asp:ListItem Value="القاهرة - الأقصر">القاهرة - الأقصر</asp:ListItem>
                                <asp:ListItem Value="القاهرة - أسوان">القاهرة - أسوان</asp:ListItem>
                                <asp:ListItem Value="القاهرة - بورسعيد">القاهرة - بورسعيد</asp:ListItem>
                                <asp:ListItem Value="القاهرة - الإسماعيلية">القاهرة - الإسماعيلية</asp:ListItem>
                                <asp:ListItem Value="القاهرة - السويس">القاهرة - السويس</asp:ListItem>
                                <asp:ListItem Value="أخرى">أخرى</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">الوصف</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" MaxLength="200" placeholder="وصف المسار"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">نقطة البداية</label>
                            <asp:DropDownList ID="ddlStartLoc" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">-- اختر نقطة البداية --</asp:ListItem>
                                <asp:ListItem Value="القاهرة">القاهرة</asp:ListItem>
                                <asp:ListItem Value="الإسكندرية">الإسكندرية</asp:ListItem>
                                <asp:ListItem Value="الجيزة">الجيزة</asp:ListItem>
                                <asp:ListItem Value="شرم الشيخ">شرم الشيخ</asp:ListItem>
                                <asp:ListItem Value="الغردقة">الغردقة</asp:ListItem>
                                <asp:ListItem Value="الأقصر">الأقصر</asp:ListItem>
                                <asp:ListItem Value="أسوان">أسوان</asp:ListItem>
                                <asp:ListItem Value="بورسعيد">بورسعيد</asp:ListItem>
                                <asp:ListItem Value="الإسماعيلية">الإسماعيلية</asp:ListItem>
                                <asp:ListItem Value="السويس">السويس</asp:ListItem>
                                <asp:ListItem Value="مرسى مطروح">مرسى مطروح</asp:ListItem>
                                <asp:ListItem Value="طنطا">طنطا</asp:ListItem>
                                <asp:ListItem Value="المنصورة">المنصورة</asp:ListItem>
                                <asp:ListItem Value="الزقازيق">الزقازيق</asp:ListItem>
                                <asp:ListItem Value="أسيوط">أسيوط</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">نقطة النهاية</label>
                            <asp:DropDownList ID="ddlEndLoc" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">-- اختر نقطة النهاية --</asp:ListItem>
                                <asp:ListItem Value="القاهرة">القاهرة</asp:ListItem>
                                <asp:ListItem Value="الإسكندرية">الإسكندرية</asp:ListItem>
                                <asp:ListItem Value="الجيزة">الجيزة</asp:ListItem>
                                <asp:ListItem Value="شرم الشيخ">شرم الشيخ</asp:ListItem>
                                <asp:ListItem Value="الغردقة">الغردقة</asp:ListItem>
                                <asp:ListItem Value="الأقصر">الأقصر</asp:ListItem>
                                <asp:ListItem Value="أسوان">أسوان</asp:ListItem>
                                <asp:ListItem Value="بورسعيد">بورسعيد</asp:ListItem>
                                <asp:ListItem Value="الإسماعيلية">الإسماعيلية</asp:ListItem>
                                <asp:ListItem Value="السويس">السويس</asp:ListItem>
                                <asp:ListItem Value="مرسى مطروح">مرسى مطروح</asp:ListItem>
                                <asp:ListItem Value="طنطا">طنطا</asp:ListItem>
                                <asp:ListItem Value="المنصورة">المنصورة</asp:ListItem>
                                <asp:ListItem Value="الزقازيق">الزقازيق</asp:ListItem>
                                <asp:ListItem Value="أسيوط">أسيوط</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">المسافة (كم)</label>
                            <asp:TextBox ID="txtKM" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" placeholder="0"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">الاتجاه</label>
                            <asp:TextBox ID="txtDirection" runat="server" CssClass="form-control" placeholder="ذهاب/إياب"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">عدد المشتركين</label>
                            <asp:TextBox ID="txtSubscriptionCount" runat="server" CssClass="form-control" TextMode="Number" min="0" placeholder="0"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">نشط</label>
                            <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" Checked="true" />
                        </div>
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-md-4">
                        <asp:Button ID="btnSaveRoute" runat="server" 
                            Text="حفظ المسار" 
                            CssClass="btn btn-save" 
                            OnClick="btnSaveRoute_Click" />
                    </div>
                    <div class="col-md-4">
                        <asp:Button ID="btnClear" runat="server" 
                            Text="مسح الحقول" 
                            CssClass="btn btn-clear" 
                            OnClick="btnClear_Click" />
                    </div>
                    <div class="col-md-4">
                        <asp:Button ID="btnBack" runat="server" 
                            Text="رجوع" 
                            CssClass="btn btn-back" 
                            OnClick="btnBack_Click" />
                    </div>
                </div>
            </asp:Panel>

            <!-- عرض المسارات للسيارة -->
            <asp:Panel ID="pnlVehicleRoutes" runat="server" Visible="false" CssClass="routes-list">
                <div class="section-header">
                    <i class="fas fa-route"></i> مسارات السيارة
                </div>

                <asp:GridView ID="gvVehicleRoutes" runat="server" 
                    CssClass="grid-view" 
                    AutoGenerateColumns="False" 
                    EmptyDataText="لا توجد مسارات لهذه السيارة"
                    ShowHeaderWhenEmpty="True"
                    AllowPaging="True"
                    PageSize="5"
                    OnPageIndexChanging="gvVehicleRoutes_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="route_name" HeaderText="اسم المسار" />
                        <asp:BoundField DataField="description" HeaderText="الوصف" />
                        <asp:BoundField DataField="start_loc" HeaderText="نقطة البداية" />
                        <asp:BoundField DataField="end_loc" HeaderText="نقطة النهاية" />
                        <asp:BoundField DataField="KM" HeaderText="المسافة (كم)" DataFormatString="{0:N0}" />
                        <asp:BoundField DataField="direction" HeaderText="الاتجاه" />
                        <asp:BoundField DataField="subscription_count" HeaderText="عدد المشتركين" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" 
                                    CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "active-badge" : "inactive-badge" %>'
                                    Text='<%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                </asp:GridView>
            </asp:Panel>

            <!-- عرض صغير للمسارات في الأسفل مع أزرار -->
            <div class="small-view">
                <div class="small-view-header">
                    <i class="fas fa-list"></i> جميع المسارات المتاحة
                    <span class="badge badge-secondary" id="totalRoutesCount">0</span>
                </div>

                <asp:Repeater ID="rptAllRoutes" runat="server" OnItemDataBound="rptAllRoutes_ItemDataBound">
                    <ItemTemplate>
                        <div class="route-item">
                            <div class="route-info">
                                <div class="route-name">
                                    <i class="fas fa-route"></i> 
                                    <asp:Label ID="lblRouteName" runat="server" Text='<%# Eval("route_name") %>'></asp:Label>
                                    <asp:Label ID="lblRouteId" runat="server" Text='<%# Eval("route_id") %>' Visible="false"></asp:Label>
                                </div>
                                <div class="route-details">
                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("description") %>'></asp:Label>
                                </div>
                                <div class="route-stats">
                                    <span class="stat-badge">
                                        <i class="fas fa-map-marker-alt"></i> 
                                        <asp:Label ID="lblStartLoc" runat="server" Text='<%# Eval("start_loc") %>'></asp:Label>
                                        <i class="fas fa-arrow-left"></i> 
                                        <asp:Label ID="lblEndLoc" runat="server" Text='<%# Eval("end_loc") %>'></asp:Label>
                                    </span>
                                    <span class="stat-badge">
                                        <i class="fas fa-road"></i> 
                                        <asp:Label ID="lblKM" runat="server" Text='<%# Eval("KM") %>'></asp:Label> كم
                                    </span>
                                    <span class="stat-badge">
                                        <i class="fas fa-exchange-alt"></i> 
                                        <asp:Label ID="lblDirection" runat="server" Text='<%# Eval("direction") %>'></asp:Label>
                                    </span>
                                    <span class="stat-badge">
                                        <i class="fas fa-users"></i> 
                                        <asp:Label ID="lblSubscriptionCount" runat="server" Text='<%# Eval("subscription_count") %>'></asp:Label> مشترك
                                    </span>
                                    <asp:Label ID="lblStatus" runat="server" 
                                        CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "active-badge" : "inactive-badge" %>'
                                        Text='<%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "غير نشط" %>'>
                                    </asp:Label>
                                </div>
                            </div>
                            <div class="route-actions">
                                <asp:LinkButton ID="btnEditRoute" runat="server" 
                                    CommandName="EditRoute" 
                                    CommandArgument='<%# Eval("route_id") %>'
                                    CssClass="btn-edit-route"
                                    OnClick="EditRoute_Click"
                                    Text="تعديل">
                                    <i class="fas fa-edit"></i> تعديل
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDeleteRoute" runat="server" 
                                    CommandName="DeleteRoute" 
                                    CommandArgument='<%# Eval("route_id") %>'
                                    CssClass="btn-delete-route"
                                    OnClick="DeleteRoute_Click"
                                    OnClientClick="return confirm('هل أنت متأكد من حذف هذا المسار؟');"
                                    Text="حذف">
                                    <i class="fas fa-trash"></i> حذف
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnLinkRoute" runat="server" 
                                    CommandName="LinkRoute" 
                                    CommandArgument='<%# Eval("route_id") %>'
                                    CssClass="btn-link-route"
                                    OnClick="LinkRoute_Click"
                                    Text="ربط بالسيارة">
                                    <i class="fas fa-link"></i> ربط بالسيارة
                                </asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                
                <asp:Label ID="lblNoRoutes" runat="server" 
                    Text="لا توجد مسارات متاحة" 
                    CssClass="text-center d-block p-3 text-muted"
                    Visible="false">
                </asp:Label>
            </div>

        </div>

    </div>

    <!-- Modal for Edit Route -->
    <div id="editModal" class="modal">
        <div class="modal-content">
            <div class="modal-header">
                <h3>تعديل المسار</h3>
                <span class="close">&times;</span>
            </div>
            <div class="modal-body">
                <input type="hidden" id="editRouteId" />
                <div class="form-group mb-3">
                    <label>اسم المسار</label>
                    <input type="text" id="editRouteName" class="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>الوصف</label>
                    <textarea id="editDescription" class="form-control" rows="2"></textarea>
                </div>
                <div class="form-group mb-3">
                    <label>نقطة البداية</label>
                    <input type="text" id="editStartLoc" class="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>نقطة النهاية</label>
                    <input type="text" id="editEndLoc" class="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>المسافة (كم)</label>
                    <input type="number" id="editKM" class="form-control" step="0.01" />
                </div>
                <div class="form-group mb-3">
                    <label>الاتجاه</label>
                    <input type="text" id="editDirection" class="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>عدد المشتركين</label>
                    <input type="number" id="editSubscriptionCount" class="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>
                        <input type="checkbox" id="editIsActive" /> نشط
                    </label>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-save" id="saveEditBtn">حفظ التعديلات</button>
                <button type="button" class="btn btn-clear" id="cancelEditBtn">إلغاء</button>
            </div>
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

        // Modal functionality
        var modal = document.getElementById('editModal');
        var span = document.getElementsByClassName('close')[0];

        function openEditModal(routeId, routeName, description, startLoc, endLoc, km, direction, subscriptionCount, isActive) {
            document.getElementById('editRouteId').value = routeId;
            document.getElementById('editRouteName').value = routeName;
            document.getElementById('editDescription').value = description;
            document.getElementById('editStartLoc').value = startLoc;
            document.getElementById('editEndLoc').value = endLoc;
            document.getElementById('editKM').value = km;
            document.getElementById('editDirection').value = direction;
            document.getElementById('editSubscriptionCount').value = subscriptionCount;
            document.getElementById('editIsActive').checked = isActive === 'True' || isActive === true;
            modal.style.display = 'block';
        }

        function closeModal() {
            modal.style.display = 'none';
        }

        span.onclick = function () {
            closeModal();
        }

        window.onclick = function (event) {
            if (event.target == modal) {
                closeModal();
            }
        }

        document.getElementById('cancelEditBtn').onclick = function () {
            closeModal();
        }

        document.getElementById('saveEditBtn').onclick = function () {
            var routeId = document.getElementById('editRouteId').value;
            var routeName = document.getElementById('editRouteName').value;
            var description = document.getElementById('editDescription').value;
            var startLoc = document.getElementById('editStartLoc').value;
            var endLoc = document.getElementById('editEndLoc').value;
            var km = document.getElementById('editKM').value;
            var direction = document.getElementById('editDirection').value;
            var subscriptionCount = document.getElementById('editSubscriptionCount').value;
            var isActive = document.getElementById('editIsActive').checked;

            __doPostBack('UpdateRoute', routeId + '|' + routeName + '|' + description + '|' + startLoc + '|' + endLoc + '|' + km + '|' + direction + '|' + subscriptionCount + '|' + isActive);
        }
    </script>

</asp:Content>