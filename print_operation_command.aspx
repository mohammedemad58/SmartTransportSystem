<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="print_operation_command.aspx.cs" 
    Inherits="car_WebApplication1.print_operation_command" %>

<!DOCTYPE html>
<html dir="rtl">
<head>
    <title>طباعة أمر الشغل</title>
    <style>
        body {
            font-family: 'Arial', 'Segoe UI', sans-serif;
            margin: 20px;
            background-color: #fff;
        }
        .print-container {
            border: 2px solid #000;
            padding: 30px;
            max-width: 800px;
            margin: 0 auto;
            background-color: #fff;
        }
        .header {
            text-align: center;
            margin-bottom: 30px;
            border-bottom: 3px solid #c8102e;
            padding-bottom: 15px;
        }
        .company-name {
            font-size: 24px;
            font-weight: bold;
            color: #c8102e;
        }
        .document-title {
            font-size: 20px;
            font-weight: bold;
            margin-top: 10px;
        }
        .work-order-info {
            background-color: #f8f9fa;
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
        }
        .info-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 15px;
            margin-bottom: 20px;
        }
        .info-item {
            margin-bottom: 10px;
        }
        .info-label {
            font-weight: bold;
            color: #333;
            margin-bottom: 5px;
        }
        .info-value {
            color: #666;
            padding: 5px 10px;
            background-color: #f8f9fa;
            border-radius: 3px;
            border: 1px solid #ddd;
        }
        .signatures {
            margin-top: 40px;
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
            text-align: center;
        }
        .signature-line {
            border-top: 1px solid #000;
            width: 80%;
            margin: 0 auto;
            padding-top: 10px;
        }
        .footer {
            text-align: center;
            margin-top: 30px;
            font-size: 12px;
            color: #666;
            border-top: 1px solid #ddd;
            padding-top: 10px;
        }
        @media print {
            body {
                margin: 0;
                padding: 0;
            }
            .print-container {
                border: none;
                padding: 0;
            }
            .no-print {
                display: none;
            }
            button {
                display: none;
            }
        }
    </style>
</head>
<body>
    <div class="print-container">
        <div class="header">
            <div class="company-name">إدارة التشغيل والنقل</div>
            <div class="document-title">أمر شغل رقم: <span id="workOrderId"></span></div>
        </div>
        
        <div class="work-order-info">
            <div style="text-align: center; font-weight: bold; margin-bottom: 10px;">
                بيانات أمر الشغل
            </div>
            <div id="workOrderDetails">
                <!-- Details will be loaded here -->
            </div>
        </div>
        
        <div class="signatures">
            <div>
                <div>اسم السائق</div>
                <div class="signature-line"></div>
            </div>
            <div>
                <div>كاتب الجراج</div>
                <div class="signature-line"></div>
            </div>
            <div>
                <div>مدير التشغيل</div>
                <div class="signature-line"></div>
            </div>
        </div>
        
        <div class="footer">
            <div>تاريخ الطباعة: <span id="printDate"></span></div>
            <div>هذا المستند تم إنشاؤه تلقائياً من نظام إدارة السيارات</div>
        </div>
    </div>
    
    <div class="no-print" style="text-align: center; margin-top: 20px;">
        <button onclick="window.print()" style="padding: 10px 20px; background-color: #c8102e; color: white; border: none; border-radius: 5px; cursor: pointer;">
            طباعة المستند
        </button>
        <button onclick="window.close()" style="padding: 10px 20px; background-color: #6c757d; color: white; border: none; border-radius: 5px; cursor: pointer; margin-left: 10px;">
            إغلاق
        </button>
    </div>
    
    <script>
        // Get work order ID from URL
        const urlParams = new URLSearchParams(window.location.search);
        const workOrderId = urlParams.get('id');
        
        if (workOrderId) {
            document.getElementById('workOrderId').textContent = workOrderId;
            
            // Load work order details via AJAX
            fetch(`print_operation_command.aspx/GetWorkOrderDetails?id=${workOrderId}`)
                .then(response => response.json())
                .then(data => {
                    if (data) {
                        document.getElementById('workOrderDetails').innerHTML = data;
                    }
                })
                .catch(error => {
                    console.error('Error loading work order details:', error);
                    document.getElementById('workOrderDetails').innerHTML = 
                        '<div style="color: red; text-align: center;">خطأ في تحميل البيانات</div>';
                });
        }
        
        // Set print date
        const now = new Date();
        document.getElementById('printDate').textContent = 
            now.toLocaleDateString('ar-EG') + ' ' + now.toLocaleTimeString('ar-EG');
            
        // Auto print after 1 second
        setTimeout(() => {
            window.print();
        }, 1000);
    </script>
</body>
</html>