<%@ Page Language="C#"
    MasterPageFile="~/Site.master"
    AutoEventWireup="true"
    CodeBehind="dashboard.aspx.cs"
    Inherits="car_WebApplication1.dashboard" %>

<asp:Content ID="TitleContent1"
    ContentPlaceHolderID="TitleContent"
    runat="server">
    لوحة التحكم - نظام إدارة الأسطول
</asp:Content>

<asp:Content ID="MainContent1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        body {
            margin: 0;
            font-family: "Segoe UI", Tahoma, sans-serif;
            background: #f5f5f5;
        }

        .header {
            background: linear-gradient(90deg, #ce1126, #ffffff, #000000);
            padding: 20px;
            text-align: center;
            color: #000;
            font-weight: bold;
            border-radius: 12px;
            margin-bottom: 20px;
        }

        /* KPI Cards */
        .kpi-container {
            max-width: 1400px;
            margin: 40px auto;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            padding: 0 20px;
        }

        .kpi-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 20px;
            padding: 20px;
            color: white;
            text-align: center;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            transition: transform 0.3s;
            cursor: pointer;
        }

        .kpi-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.15);
        }

        .kpi-card:nth-child(1) { background: linear-gradient(135deg, #ce1126, #ff6b6b); }
        .kpi-card:nth-child(2) { background: linear-gradient(135deg, #28a745, #20c997); }
        .kpi-card:nth-child(3) { background: linear-gradient(135deg, #17a2b8, #4ecdc4); }
        .kpi-card:nth-child(4) { background: linear-gradient(135deg, #fd7e14, #ffc107); }
        .kpi-card:nth-child(5) { background: linear-gradient(135deg, #6f42c1, #a855f7); }
        .kpi-card:nth-child(6) { background: linear-gradient(135deg, #e83e8c, #f43f5e); }
        .kpi-card:nth-child(7) { background: linear-gradient(135deg, #4a5568, #718096); }
        .kpi-card:nth-child(8) { background: linear-gradient(135deg, #38b2ac, #319795); }

        .kpi-icon {
            font-size: 35px;
            margin-bottom: 15px;
        }

        .kpi-value {
            font-size: 32px;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .kpi-label {
            font-size: 14px;
            opacity: 0.9;
            margin-bottom: 10px;
        }

        .kpi-trend {
            font-size: 12px;
            padding: 4px 8px;
            border-radius: 20px;
            display: inline-block;
        }

        .kpi-trend.positive {
            background: rgba(255,255,255,0.3);
            color: #d4edda;
        }

        .kpi-trend.negative {
            background: rgba(255,255,255,0.3);
            color: #f8d7da;
        }

        .kpi-trend.neutral {
            background: rgba(255,255,255,0.3);
            color: #d1ecf1;
        }

        /* Charts Container */
        .charts-container {
            max-width: 1400px;
            margin: 40px auto;
            padding: 20px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
            gap: 25px;
        }

        .chart-card {
            background: white;
            border-radius: 20px;
            padding: 20px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            transition: transform 0.2s;
        }

        .chart-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.15);
        }

        .chart-card-full {
            max-width: 1400px;
            margin: 40px auto;
            background: white;
            border-radius: 20px;
            padding: 20px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }

        .chart-title {
            text-align: center;
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 20px;
            color: #333;
            padding-bottom: 10px;
            border-bottom: 2px solid #f0f0f0;
        }

        .chart-wrapper {
            position: relative;
            height: 250px;
            width: 100%;
            margin: 0 auto;
        }

        .chart-wrapper-large {
            position: relative;
            height: 350px;
            width: 100%;
        }

        .chart-legend {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            gap: 15px;
            margin-top: 20px;
            padding-top: 15px;
            border-top: 1px solid #f0f0f0;
        }

        .legend-item {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 12px;
        }

        .legend-color {
            width: 12px;
            height: 12px;
            border-radius: 4px;
        }

        .stats-badge {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
            margin-left: 10px;
        }

        .badge-red { background: #ce1126; color: white; }
        .badge-white { background: #f0f0f0; color: #333; border: 1px solid #ddd; }
        .badge-black { background: #333; color: white; }
        .badge-green { background: #28a745; color: white; }
        .badge-blue { background: #17a2b8; color: white; }

        .alert-warning {
            background: #fff3cd;
            border: 1px solid #ffc107;
            color: #856404;
            padding: 10px;
            border-radius: 8px;
            margin: 20px auto;
            max-width: 1400px;
        }
    </style>

    <div class="header">
        <h2>لوحة التحكم - نظام إدارة الأسطول</h2>
        <div>
            مرحباً، <%= Session["username"] %>
        </div>
    </div>



    <!-- Alert for expiring subscriptions -->
    <div id="expiringAlert" class="alert-warning" style="display: none; text-align: center;">
        <i class="fas fa-exclamation-triangle"></i> 
        يوجد <span id="expiringCount">0</span> اشتراك على وشك الانتهاء خلال 30 يوماً!
    </div>

    <!-- Charts Section -->
    <div class="charts-container">
        <!-- Chart 1: Employee Distribution -->
        <div class="chart-card">
            <div class="chart-title">
                توزيع الموظفين حسب الوظيفة
                <span class="stats-badge badge-red">إجمالي: <asp:Label ID="lblTotalEmployees" runat="server" Text="0"></asp:Label></span>
            </div>
            <div class="chart-wrapper">
                <canvas id="employeeChart"></canvas>
            </div>
            <div class="chart-legend" id="employeeLegend"></div>
        </div>

        <!-- Chart 2: Vehicle Type Distribution -->
        <div class="chart-card">
            <div class="chart-title">
                توزيع المركبات حسب النوع
                <span class="stats-badge badge-white">إجمالي: <asp:Label ID="lblTotalVehicles" runat="server" Text="0"></asp:Label></span>
            </div>
            <div class="chart-wrapper">
                <canvas id="vehicleTypeChart"></canvas>
            </div>
            <div class="chart-legend" id="vehicleLegend"></div>
        </div>

        <!-- Chart 3: Vehicle Status Distribution -->
        <div class="chart-card">
            <div class="chart-title">
                حالة المركبات
                <span class="stats-badge badge-black">الأسطول</span>
            </div>
            <div class="chart-wrapper">
                <canvas id="vehicleStatusChart"></canvas>
            </div>
            <div class="chart-legend" id="vehicleStatusLegend"></div>
        </div>

        <!-- Chart 4: Top Performing Routes -->
        <div class="chart-card">
            <div class="chart-title">
                أفضل المسارات من حيث عدد المركبات
                <span class="stats-badge badge-green">أفضل 5</span>
            </div>
            <div class="chart-wrapper">
                <canvas id="routesChart"></canvas>
            </div>
            <div class="chart-legend" id="routesLegend"></div>
        </div>

        <!-- Chart 5: Maintenance Requests by Type -->
        <div class="chart-card">
            <div class="chart-title">
                طلبات الصيانة حسب النوع
                <span class="stats-badge badge-blue">إجمالي: <asp:Label ID="lblTotalMaintenance" runat="server" Text="0"></asp:Label></span>
            </div>
            <div class="chart-wrapper">
                <canvas id="maintenanceChart"></canvas>
            </div>
            <div class="chart-legend" id="maintenanceLegend"></div>
        </div>

        <!-- Chart 6: Driver Performance -->
        <div class="chart-card">
            <div class="chart-title">
                أداء السائقين (أكثر السائقين نشاطاً)
                <span class="stats-badge badge-white">شهر جاري</span>
            </div>
            <div class="chart-wrapper">
                <canvas id="driverChart"></canvas>
            </div>
            <div class="chart-legend" id="driverLegend"></div>
        </div>
    </div>



    <!-- Hidden fields to store chart data -->
    <asp:HiddenField ID="hdnEmployeeData" runat="server" />
    <asp:HiddenField ID="hdnVehicleData" runat="server" />
    <asp:HiddenField ID="hdnMaintenanceData" runat="server" />
    <asp:HiddenField ID="hdnRoutePerformance" runat="server" />
    <asp:HiddenField ID="hdnDriverPerformance" runat="server" />
    <asp:HiddenField ID="hdnVehicleStatus" runat="server" />
    <asp:HiddenField ID="hdnMonthlyTrends" runat="server" />

    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    
    <!-- Chart.js library -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    
    <script>
        document.addEventListener('DOMContentLoaded', function () {

            // Chart 1: Employee Distribution
            const employeeData = JSON.parse(document.getElementById('<%= hdnEmployeeData.ClientID %>').value || '{"labels":[],"values":[],"colors":[],"total":0}');
            if (employeeData.labels.length > 0) {
                const ctx1 = document.getElementById('employeeChart').getContext('2d');
                new Chart(ctx1, {
                    type: 'doughnut',
                    data: {
                        labels: employeeData.labels,
                        datasets: [{
                            data: employeeData.values,
                            backgroundColor: employeeData.colors,
                            borderWidth: 0,
                            borderRadius: 10,
                            spacing: 5
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        cutout: '65%',
                        plugins: {
                            legend: { display: false },
                            tooltip: {
                                backgroundColor: 'rgba(0,0,0,0.8)',
                                callbacks: {
                                    label: function (context) {
                                        const total = employeeData.total;
                                        const value = context.raw;
                                        const percentage = ((value / total) * 100).toFixed(1);
                                        return `${context.label}: ${value} موظف (${percentage}%)`;
                                    }
                                }
                            }
                        }
                    }
                });

                // Populate legend
                const employeeLegend = document.getElementById('employeeLegend');
                employeeLegend.innerHTML = '';
                for (let i = 0; i < employeeData.labels.length; i++) {
                    const total = employeeData.total;
                    const value = employeeData.values[i];
                    const percentage = ((value / total) * 100).toFixed(1);
                    const legendItem = document.createElement('div');
                    legendItem.className = 'legend-item';
                    legendItem.innerHTML = `
                        <span class="legend-color" style="background: ${employeeData.colors[i]};"></span>
                        <span>${employeeData.labels[i]}: ${value} (${percentage}%)</span>
                    `;
                    employeeLegend.appendChild(legendItem);
                }
            }

            // Chart 2: Vehicle Type Distribution
            const vehicleData = JSON.parse(document.getElementById('<%= hdnVehicleData.ClientID %>').value || '{"labels":[],"values":[],"colors":[],"total":0}');
            if (vehicleData.labels.length > 0) {
                const ctx2 = document.getElementById('vehicleTypeChart').getContext('2d');
                new Chart(ctx2, {
                    type: 'doughnut',
                    data: {
                        labels: vehicleData.labels,
                        datasets: [{
                            data: vehicleData.values,
                            backgroundColor: vehicleData.colors,
                            borderWidth: 0,
                            borderRadius: 10,
                            spacing: 5
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        cutout: '65%',
                        plugins: {
                            legend: { display: false },
                            tooltip: {
                                callbacks: {
                                    label: function (context) {
                                        const total = vehicleData.total;
                                        const value = context.raw;
                                        const percentage = ((value / total) * 100).toFixed(1);
                                        return `${context.label}: ${value} سيارة (${percentage}%)`;
                                    }
                                }
                            }
                        }
                    }
                });

                const vehicleLegend = document.getElementById('vehicleLegend');
                vehicleLegend.innerHTML = '';
                for (let i = 0; i < vehicleData.labels.length; i++) {
                    const total = vehicleData.total;
                    const value = vehicleData.values[i];
                    const percentage = ((value / total) * 100).toFixed(1);
                    const legendItem = document.createElement('div');
                    legendItem.className = 'legend-item';
                    legendItem.innerHTML = `
                        <span class="legend-color" style="background: ${vehicleData.colors[i]};"></span>
                        <span>${vehicleData.labels[i]}: ${value} (${percentage}%)</span>
                    `;
                    vehicleLegend.appendChild(legendItem);
                }
            }

            // Chart 3: Vehicle Status Distribution
            const statusData = JSON.parse(document.getElementById('<%= hdnVehicleStatus.ClientID %>').value || '{"labels":[],"values":[],"colors":[],"total":0}');
            if (statusData.labels.length > 0) {
                const ctx3 = document.getElementById('vehicleStatusChart').getContext('2d');
                new Chart(ctx3, {
                    type: 'pie',
                    data: {
                        labels: statusData.labels,
                        datasets: [{
                            data: statusData.values,
                            backgroundColor: statusData.colors,
                            borderWidth: 0
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { display: false },
                            tooltip: {
                                callbacks: {
                                    label: function(context) {
                                        const total = statusData.total;
                                        const value = context.raw;
                                        const percentage = ((value / total) * 100).toFixed(1);
                                        return `${context.label}: ${value} سيارة (${percentage}%)`;
                                    }
                                }
                            }
                        }
                    }
                });

                const statusLegend = document.getElementById('vehicleStatusLegend');
                statusLegend.innerHTML = '';
                for (let i = 0; i < statusData.labels.length; i++) {
                    const total = statusData.total;
                    const value = statusData.values[i];
                    const percentage = ((value / total) * 100).toFixed(1);
                    const legendItem = document.createElement('div');
                    legendItem.className = 'legend-item';
                    legendItem.innerHTML = `
                        <span class="legend-color" style="background: ${statusData.colors[i]};"></span>
                        <span>${statusData.labels[i]}: ${value} (${percentage}%)</span>
                    `;
                    statusLegend.appendChild(legendItem);
                }
            }

            // Chart 4: Routes Performance
            const routesData = JSON.parse(document.getElementById('<%= hdnRoutePerformance.ClientID %>').value || '{"labels":[],"values":[],"subscribers":[]}');
            if (routesData.labels.length > 0) {
                const ctx4 = document.getElementById('routesChart').getContext('2d');
                new Chart(ctx4, {
                    type: 'bar',
                    data: {
                        labels: routesData.labels,
                        datasets: [{
                            label: 'عدد المركبات',
                            data: routesData.values,
                            backgroundColor: '#ce1126',
                            borderRadius: 10
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { position: 'top' },
                            tooltip: {
                                callbacks: {
                                    afterBody: function(context) {
                                        const index = context[0].dataIndex;
                                        return `المشتركين: ${routesData.subscribers[index] || 0}`;
                                    }
                                }
                            }
                        }
                    }
                });

                const routesLegend = document.getElementById('routesLegend');
                routesLegend.innerHTML = '<span style="font-size:12px;">أكثر المسارات نشاطاً حسب عدد المركبات</span>';
            }

            // Chart 5: Maintenance Distribution
            const maintenanceData = JSON.parse(document.getElementById('<%= hdnMaintenanceData.ClientID %>').value || '{"labels":[],"values":[],"colors":[],"total":0}');
            if (maintenanceData.labels.length > 0) {
                const ctx5 = document.getElementById('maintenanceChart').getContext('2d');
                new Chart(ctx5, {
                    type: 'doughnut',
                    data: {
                        labels: maintenanceData.labels,
                        datasets: [{
                            data: maintenanceData.values,
                            backgroundColor: maintenanceData.colors,
                            borderWidth: 0,
                            borderRadius: 10,
                            spacing: 5
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        cutout: '65%',
                        plugins: {
                            legend: { display: false },
                            tooltip: {
                                callbacks: {
                                    label: function(context) {
                                        const total = maintenanceData.total;
                                        const value = context.raw;
                                        const percentage = ((value / total) * 100).toFixed(1);
                                        return `${context.label}: ${value} طلب (${percentage}%)`;
                                    }
                                }
                            }
                        }
                    }
                });

                const maintenanceLegend = document.getElementById('maintenanceLegend');
                maintenanceLegend.innerHTML = '';
                for (let i = 0; i < maintenanceData.labels.length; i++) {
                    const total = maintenanceData.total;
                    const value = maintenanceData.values[i];
                    const percentage = ((value / total) * 100).toFixed(1);
                    const legendItem = document.createElement('div');
                    legendItem.className = 'legend-item';
                    legendItem.innerHTML = `
                        <span class="legend-color" style="background: ${maintenanceData.colors[i]};"></span>
                        <span>${maintenanceData.labels[i]}: ${value} (${percentage}%)</span>
                    `;
                    maintenanceLegend.appendChild(legendItem);
                }
            }

            // Chart 6: Driver Performance
            const driverData = JSON.parse(document.getElementById('<%= hdnDriverPerformance.ClientID %>').value || '{"labels":[],"values":[]}');
            if (driverData.labels.length > 0) {
                const ctx6 = document.getElementById('driverChart').getContext('2d');
                new Chart(ctx6, {
                    type: 'bar',
                    data: {
                        labels: driverData.labels,
                        datasets: [{
                            label: 'عدد الرحلات',
                            data: driverData.values,
                            backgroundColor: '#28a745',
                            borderRadius: 10
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        indexAxis: 'y',
                        plugins: {
                            legend: { position: 'top' }
                        }
                    }
                });

                const driverLegend = document.getElementById('driverLegend');
                driverLegend.innerHTML = '<span style="font-size:12px;">أكثر السائقين نشاطاً خلال الشهر الحالي</span>';
            }

            // Chart 7: Monthly Trends
            const trendsData = JSON.parse(document.getElementById('<%= hdnMonthlyTrends.ClientID %>').value || '{"labels":[],"vehicles":[],"subscribers":[]}');
            if (trendsData.labels.length > 0) {
                const ctx7 = document.getElementById('trendsChart').getContext('2d');
                new Chart(ctx7, {
                    type: 'line',
                    data: {
                        labels: trendsData.labels,
                        datasets: [
                            {
                                label: 'المركبات النشطة',
                                data: trendsData.vehicles,
                                borderColor: '#ce1126',
                                backgroundColor: 'rgba(206, 17, 38, 0.1)',
                                tension: 0.4,
                                fill: true
                            },
                            {
                                label: 'المشتركين الجدد',
                                data: trendsData.subscribers,
                                borderColor: '#28a745',
                                backgroundColor: 'rgba(40, 167, 69, 0.1)',
                                tension: 0.4,
                                fill: true
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        interaction: { mode: 'index', intersect: false },
                        plugins: {
                            tooltip: { enabled: true }
                        }
                    }
                });
            }
        });

        // Animation on scroll
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }
            });
        });

        document.querySelectorAll('.chart-card, .chart-card-full, .kpi-card').forEach((card) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';
            card.style.transition = 'opacity 0.5s, transform 0.5s';
            observer.observe(card);
        });
    </script>

</asp:Content>