<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.master"
    CodeBehind="subscribers_management.aspx.cs"
    Inherits="car_WebApplication1.SubscribersManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .page-title { font-weight: bold; color: #000; text-align: center; margin-bottom: 30px; font-size: 32px; }
    .main-box { border: 2px solid #000; border-radius: 14px; padding: 30px; background-color: #fff; margin-bottom: 20px; }
    .section-header { background: linear-gradient(90deg, #ce1126, #ffffff); padding: 12px 18px; border-radius: 8px; margin-bottom: 20px; font-weight: bold; color: #000; overflow: auto; font-size: 18px; }
    .form-label { font-weight: bold; font-size: 15px; margin-bottom: 6px; }
    .required-field::after { content: " *"; color: #ce1126; font-weight: bold; }
    .filter-info { background: #f0f0f0; padding: 10px 15px; border-radius: 8px; margin-bottom: 15px; font-size: 14px; color: #666; }
    .filter-info strong { color: #a50d25; }

    .btn-save { background-color: #a50d25; border-color: #c8102e; color: white; font-weight: bold; height: 52px; width: 100%; font-size: 17px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-save:hover { background-color: #8a0a1e; }
    .btn-clear { background-color: #6c757d; color: #fff; font-weight: bold; height: 52px; width: 100%; font-size: 17px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-clear:hover { background-color: #5a6268; }
    .btn-add { background-color: #28a745; color: white; font-weight: bold; height: 42px; width: 100%; font-size: 15px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-add:hover { background-color: #218838; }
    .btn-remove { background-color: #dc3545; color: white; border: none; padding: 3px 10px; border-radius: 4px; cursor: pointer; font-size: 12px; }
    .btn-remove:hover { background-color: #c82333; }
    .btn-search { background-color: #0d6efd; color: white; font-weight: bold; height: 42px; width: 100%; font-size: 16px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-search:hover { background-color: #0b5ed7; }
    .btn-reset { background-color: #6c757d; color: white; font-weight: bold; height: 42px; width: 100%; font-size: 16px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-reset:hover { background-color: #5a6268; }

    .tab-container { text-align: center; margin-bottom: 20px; border-bottom: 2px solid #dee2e6; }
    .tab-btn { background: transparent; border: none; color: #495057; font-weight: bold; padding: 12px 30px; margin: 0 2px; border-radius: 0; cursor: pointer; font-size: 16px; transition: all 0.3s; }
    .tab-btn:hover { color: #a50d25; }
    .tab-btn.active { color: #a50d25; border-bottom: 3px solid #c8102e; background-color: transparent; }

    .tab-content { display: none; animation: fadeIn 0.3s; }
    .tab-content.active { display: block; }

    @keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }

    .grid-view { width: 100%; border-collapse: collapse; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
    .grid-view th { background-color: #a50d25; color: white; font-weight: bold; padding: 14px; text-align: center; font-size: 14px; }
    .grid-view td { padding: 12px; border-bottom: 1px solid #dee2e6; text-align: center; font-size: 14px; }
    .grid-view tr:hover { background-color: #f5f5f5; }
    .grid-view .cost-column { color: #a50d25; font-weight: bold; }

    .pagination-ys { background: #f8f9fa; padding: 10px; text-align: center; border-radius: 0 0 10px 10px; }
    .pagination-ys a, .pagination-ys span { display: inline-block; padding: 5px 10px; margin: 0 2px; border: 1px solid #dee2e6; border-radius: 4px; text-decoration: none; color: #a50d25; font-weight: bold; }
    .pagination-ys a:hover { background-color: #a50d25; color: white; }
    .pagination-ys span { background-color: #a50d25; color: white; }

    .form-control { font-size: 15px; padding: 10px 14px; height: 42px; }
    select.form-control { height: 42px; padding: 8px 12px; }

    .alert { font-size: 16px; padding: 15px 20px; }

    .step-indicator { display: flex; gap: 10px; margin-bottom: 20px; flex-wrap: wrap; }
    .step { background: #f8f9fa; padding: 8px 15px; border-radius: 20px; font-size: 13px; color: #6c757d; }
    .step.active { background: #a50d25; color: white; }
    .step.completed { background: #d4edda; color: #155724; }

    .search-box { background: #f8f9fa; border: 1px solid #dee2e6; border-radius: 10px; padding: 20px; margin-bottom: 20px; }

    .status-badge-active { background-color: #d4edda; padding: 3px 10px; border-radius: 12px; font-size: 12px; color: #155724; }
    .status-badge-expired { background-color: #f8d7da; padding: 3px 10px; border-radius: 12px; font-size: 12px; color: #721c24; }
    .status-badge-upcoming { background-color: #fff3cd; padding: 3px 10px; border-radius: 12px; font-size: 12px; color: #856404; }

    .badge-count { background-color: #a50d25; color: white; border-radius: 50%; padding: 2px 8px; margin-right: 5px; font-size: 12px; }

    /* Selected subscribers list */
    .selected-list { background: #f8f9fa; border: 1px solid #dee2e6; border-radius: 8px; padding: 15px; margin-top: 10px; max-height: 250px; overflow-y: auto; }
    .selected-item { display: flex; justify-content: space-between; align-items: center; background: white; padding: 8px 12px; margin-bottom: 6px; border-radius: 5px; border-right: 3px solid #28a745; }
    .selected-item:last-child { margin-bottom: 0; }
    .selected-item .name { font-weight: bold; color: #333; }
    .empty-selected { text-align: center; color: #999; padding: 20px; }

    @media (max-width: 768px) {
        .page-title { font-size: 24px; }
        .main-box { padding: 15px !important; }
        .grid-view th, .grid-view td { font-size: 12px !important; padding: 8px !important; }
        .btn-save, .btn-clear { height: 46px !important; font-size: 15px !important; }
        .section-header { font-size: 15px !important; padding: 10px 14px !important; }
        .form-control { font-size: 14px !important; height: 38px !important; padding: 8px 12px !important; }
        .step-indicator { gap: 5px; }
        .step { font-size: 11px; padding: 5px 10px; }
        .search-box { padding: 15px; }
        .btn-search, .btn-reset { height: 38px !important; font-size: 14px !important; }
        .tab-btn { padding: 10px 15px; font-size: 14px; }
    }
</style>

<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

<div class="container mt-4" dir="rtl">
    <div class="text-center mb-4">
        <h2 class="page-title">اشتراكات</h2>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
    <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

    <!-- ===== TABS ===== -->
    <div class="tab-container">
        <asp:Button ID="btnTabEntry" runat="server" Text="إضافة اشتراك جديد" CssClass="tab-btn active" OnClick="BtnTabEntry_Click" CausesValidation="false" />
        <asp:Button ID="btnTabSearch" runat="server" Text="بحث في الاشتراكات" CssClass="tab-btn" OnClick="BtnTabSearch_Click" CausesValidation="false" />
    </div>

    <!-- ===== TAB 1: ENTRY ===== -->
    <div id="tabEntry" class="tab-content active">
        <div class="main-box">
            <div class="section-header">
                <i class="fas fa-users"></i> إضافة اشتراك جديد
            </div>

          

           

            <div class="row g-3">
                <!-- 1) الخط -->
                <div class="col-md-12">
                    <label class="form-label required-field">الخط</label>
                    <asp:DropDownList ID="ddlRoute" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRoute_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <!-- 2) المشتركين - Add Button -->
                <div class="col-md-9">
                    <label class="form-label required-field">المشترك</label>
                    <asp:DropDownList ID="ddlSubscriber" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <label class="form-label">&nbsp;</label>
                    <asp:Button ID="btnAddSubscriber" runat="server" Text="+ إضافة اشتراكات" CssClass="btn-add" OnClick="BtnAddSubscriber_Click" CausesValidation="false" />
                </div>

                <!-- Selected Subscribers List -->
                <div class="col-md-12">
                    <label class="form-label">
                        المشتركين المضافة
                        <span class="badge-count" id="selectedCount">0</span>
                    </label>
                    <div class="selected-list" id="selectedListContainer">
                        <asp:Repeater ID="rptSelectedSubscribers" runat="server" OnItemCommand="RptSelectedSubscribers_ItemCommand">
                            <ItemTemplate>
                                <div class="selected-item">
                                    <span class="name"><i class="fas fa-user"></i> <%# Eval("Name") %></span>
                                    <asp:LinkButton ID="btnRemove" runat="server"
                                        CommandName="RemoveSubscriber"
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn-remove"
                                        CausesValidation="false">
                                        <i class="fas fa-times"></i> إزالة
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlNoSelected" runat="server" CssClass="empty-selected">
                            <i class="fas fa-info-circle"></i> لم تتم إضافة أي مشترك بعد
                        </asp:Panel>
                    </div>
                </div>

                <!-- 3) السيارة -->
                <div class="col-md-6">
                    <label class="form-label required-field">السيارة</label>
                    <asp:DropDownList ID="ddlCarNumber" runat="server" CssClass="form-control">
                        <asp:ListItem Value="0">-- اختر السيارة --</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- 4) السائق -->
                <div class="col-md-6">
                    <label class="form-label required-field">السائق</label>
                    <asp:DropDownList ID="ddlDriver" runat="server" CssClass="form-control">
                        <asp:ListItem Value="0">-- اختر السائق --</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <!-- Route details -->
            <asp:Panel ID="pnlRouteDetails" runat="server" Visible="false" CssClass="mt-3">
                <div class="alert alert-info">
                    <strong>تفاصيل الخط:</strong><br />
                    <asp:Label ID="lblRouteDetails" runat="server"></asp:Label>
                </div>
            </asp:Panel>

            <div class="row g-3 mt-1">
                <div class="col-md-4">
                    <label class="form-label">تاريخ البداية</label>
                    <asp:TextBox ID="txtSubStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label">تاريخ النهاية</label>
                    <asp:TextBox ID="txtSubEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
             
                <div class="col-md-12">
                    <label class="form-label">ملاحظات</label>
                    <asp:TextBox ID="txtSubNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
                </div>
            </div>

            <div class="row mt-4">
                <div class="col-md-6">
                    <asp:Button ID="btnSaveSubscriber" runat="server" Text="حفظ الاشتراكات" CssClass="btn btn-save" OnClick="BtnSaveSubscriber_Click" />
                </div>
                <div class="col-md-6">
                    <asp:Button ID="btnClearSubscriber" runat="server" Text="مسح الحقول" CssClass="btn btn-clear" OnClick="BtnClearSubscriber_Click" />
                </div>
            </div>

            <!-- History Section -->
            <div class="mt-4">
                <div class="section-header">
                    <i class="fas fa-history"></i> سجل الاشتراكات (جميع الاشتراكات)
                    <span class="badge-count" id="historyCount">0</span>
                </div>
                <asp:GridView ID="gvSubscriberHistory" runat="server"
                    CssClass="grid-view"
                    AutoGenerateColumns="false"
                    EmptyDataText="لا توجد اشتراكات مسجلة"
                    AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="GvSubscriberHistory_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="subscriber_name" HeaderText="اسم المشتركين" />
                        <asp:BoundField DataField="route_name" HeaderText="الخط" />
                        <asp:BoundField DataField="car_number" HeaderText="السيارة" />
                        <asp:BoundField DataField="driver_name" HeaderText="السائق" />
                        <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                        
                    </Columns>
                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- ===== TAB 2: SEARCH ===== -->
    <div id="tabSearch" class="tab-content">
        <div class="main-box">
            <div class="section-header">
                <i class="fas fa-search"></i> البحث في سجل الاشتراكات
            </div>

            <div class="search-box">
                <div class="row g-3">
                    <div class="col-md-3">
                        <label class="form-label">اسم المشترك</label>
                        <asp:TextBox ID="txtSearchSubscriber" runat="server" CssClass="form-control" placeholder="بحث باسم المشترك..."></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">الخط</label>
                        <asp:DropDownList ID="ddlSearchRoute" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- الكل --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">رقم السيارة</label>
                        <asp:TextBox ID="txtSearchCar" runat="server" CssClass="form-control" placeholder="بحث برقم السيارة..."></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">السائق</label>
                        <asp:TextBox ID="txtSearchDriver" runat="server" CssClass="form-control" placeholder="بحث باسم السائق..."></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">تاريخ البداية من</label>
                        <asp:TextBox ID="txtSearchStartDateFrom" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">تاريخ البداية إلى</label>
                        <asp:TextBox ID="txtSearchStartDateTo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">الحالة</label>
                        <asp:DropDownList ID="ddlSearchStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- الكل --</asp:ListItem>
                            <asp:ListItem Value="active">نشط</asp:ListItem>
                            <asp:ListItem Value="expired">منتهي</asp:ListItem>
                            <asp:ListItem Value="upcoming">لم يبدأ</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">&nbsp;</label>
                        <div class="row g-2">
                            <div class="col-6">
                                <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn-search w-100" OnClick="BtnSearch_Click" />
                            </div>
                            <div class="col-6">
                                <asp:Button ID="btnResetSearch" runat="server" Text="إعادة تعيين" CssClass="btn-reset w-100" OnClick="BtnResetSearch_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="mt-3">
                <div class="section-header" style="font-size: 15px;">
                    <i class="fas fa-list"></i> نتائج البحث
                    <span class="badge-count" id="searchResultCount">0</span>
                </div>

                <asp:GridView ID="gvSearchResults" runat="server"
                    CssClass="grid-view"
                    AutoGenerateColumns="false"
                    EmptyDataText="لا توجد نتائج مطابقة للبحث"
                    AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="GvSearchResults_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="subscriber_name" HeaderText="اسم المشترك" />
                        <asp:BoundField DataField="route_name" HeaderText="الخط" />
                        <asp:BoundField DataField="car_number" HeaderText="السيارة" />
                        <asp:BoundField DataField="driver_name" HeaderText="السائق" />
                        <asp:BoundField DataField="start_date" HeaderText="تاريخ البداية" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="end_date" HeaderText="تاريخ النهاية" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="cost" HeaderText="التكلفة" DataFormatString="{0:N2}" ItemStyle-CssClass="cost-column" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server"
                                    CssClass='<%# GetStatusCss(Convert.ToDateTime(Eval("start_date")), Convert.ToDateTime(Eval("end_date"))) %>'
                                    Text='<%# GetStatusText(Convert.ToDateTime(Eval("start_date")), Convert.ToDateTime(Eval("end_date"))) %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">
    function showTab(tabName) {
        var tabs = document.getElementsByClassName('tab-content');
        for (var i = 0; i < tabs.length; i++) {
            tabs[i].classList.remove('active');
        }

        var buttons = document.getElementsByClassName('tab-btn');
        for (var i = 0; i < buttons.length; i++) {
            buttons[i].classList.remove('active');
        }

        var selectedTab = document.getElementById('tab' + tabName);
        if (selectedTab) {
            selectedTab.classList.add('active');
        }

        if (event && event.target) {
            event.target.classList.add('active');
        }
    }

    function updateStepIndicator(step) {
        for (var i = 1; i <= 4; i++) {
            var el = document.getElementById('step' + i);
            if (el) {
                el.classList.remove('active', 'completed');
                if (i < step) el.classList.add('completed');
                else if (i === step) el.classList.add('active');
            }
        }
    }

    function updateFilterInfo(text) {
        var el = document.getElementById('filterDetails');
        if (el) el.innerHTML = text;
    }
</script>

</asp:Content>