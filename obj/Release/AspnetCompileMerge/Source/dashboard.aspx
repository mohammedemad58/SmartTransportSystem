<%@ Page Language="C#"
    MasterPageFile="~/Site.master"
    AutoEventWireup="true"
    CodeBehind="dashboard.aspx.cs"
    Inherits="car_WebApplication1.dashboard" %>

<asp:Content ID="TitleContent1"
    ContentPlaceHolderID="TitleContent"
    runat="server">
    لوحة التحكم
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

        .dashboard {
            max-width: 900px;
            margin: 40px auto;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
        }

        .dash-btn {
            height: 130px;
            border-radius: 16px;
            font-size: 18px;
            font-weight: bold;
            display: flex;
            align-items: center;
            justify-content: center;
            text-decoration: none;
            transition: transform 0.2s;
        }

        .dash-btn:hover {
            transform: scale(1.02);
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
        }

        .red { background: #ce1126; color: #fff; }
        .white { background: #fff; color: #000; border: 2px solid #000; }
        .black { background: #000; color: #fff; }

        /* Charts Container */
        .charts-container {
            max-width: 1200px;
            margin: 40px auto;
            padding: 20px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
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
            font-size: 14px;
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
            margin-left: 5px;
        }

        .badge-red { background: #ce1126; color: white; }
        .badge-white { background: #f0f0f0; color: #333; border: 1px solid #ddd; }
        .badge-black { background: #333; color: white; }
    </style>

    <div class="header">
        <h2>لوحة التحكم</h2>
        <div>
            مرحباً، <%= Session["username"] %>
        </div>
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

        <!-- Chart 3: Maintenance Requests by Type -->
        <div class="chart-card">
            <div class="chart-title">
                طلبات الصيانة حسب النوع
                <span class="stats-badge badge-black">إجمالي: <asp:Label ID="lblTotalMaintenance" runat="server" Text="0"></asp:Label></span>
            </div>
            <div class="chart-wrapper">
                <canvas id="maintenanceChart"></canvas>
            </div>
            <div class="chart-legend" id="maintenanceLegend"></div>
        </div>
    </div>

    <!-- Hidden fields to store chart data -->
    <asp:HiddenField ID="hdnEmployeeData" runat="server" />
    <asp:HiddenField ID="hdnVehicleData" runat="server" />
    <asp:HiddenField ID="hdnMaintenanceData" runat="server" />

    <!-- Add Chart.js library -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    
    <script>
        document.addEventListener('DOMContentLoaded', function () {

            // Chart 1: Employee Distribution (Dynamic from database)
            const employeeData = JSON.parse(document.getElementById('<%= hdnEmployeeData.ClientID %>').value);
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
                            titleFont: { size: 14 },
                            bodyFont: { size: 13 },
                            padding: 12,
                            cornerRadius: 8,
                            callbacks: {
                                label: function(context) {
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

            // Populate legend for Employee chart
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

            // Chart 2: Vehicle Type Distribution (Dynamic from database)
            const vehicleData = JSON.parse(document.getElementById('<%= hdnVehicleData.ClientID %>').value);
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
                            backgroundColor: 'rgba(0,0,0,0.8)',
                            callbacks: {
                                label: function(context) {
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

            // Populate legend for Vehicle chart
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

            // Chart 3: Maintenance Requests by Type (Dynamic from database)
            const maintenanceData = JSON.parse(document.getElementById('<%= hdnMaintenanceData.ClientID %>').value);
            const ctx3 = document.getElementById('maintenanceChart').getContext('2d');

            new Chart(ctx3, {
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
                            backgroundColor: 'rgba(0,0,0,0.8)',
                            titleFont: { size: 14 },
                            bodyFont: { size: 13 },
                            padding: 12,
                            cornerRadius: 8,
                            callbacks: {
                                label: function (context) {
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

            // Populate legend for Maintenance chart
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
        });
    </script>

    <!-- Animation on scroll -->
    <script>
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }
            });
        });

        document.querySelectorAll('.chart-card').forEach((card) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';
            card.style.transition = 'opacity 0.5s, transform 0.5s';
            observer.observe(card);
        });
    </script>

</asp:Content>