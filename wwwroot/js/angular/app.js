
var app = angular.module('app', []);

app.controller('MyController', function ($scope, $http, $window, $filter) {
    // List for Dropdowns

    $scope.QuoData = {
        QuoNumber: '',
        CustomerName: '',
        CustFirstname: '',
        CustLastname: '',
        CompanyName: '',
        OrderDate: new Date().toISOString().slice(0, 10), // รูปแบบ yyyy-MM-dd
        OrderStatus: 'Processing', //กำหนดให้เป็น Processing
        ShipDate: new Date(new Date().setDate(new Date().getDate() + 3)).toISOString().slice(0, 10), // วันที่ปัจจุบัน + 3 วันในรูปแบบ yyyy-MM-dd
        TotalQty: 0,
        TotalPrice: 0,
        CustomerEmail: '',
        CustomerAddress: '',
        CustomerTaxID: '',
        CustomerAddressTax: '',
        CustomerPhone: '',
        Remark: '',
        CreateBy: 'ADMIN',
        CreateDate: new Date().toISOString().slice(0, 10),// รูปแบบ dd/MM/yyyy HH:mm:ss
        StyleName: '',// ไม่มี MAP    
        QuoType: '',
        QuoLastname: '',
        QuoCompanyName: '',
        QuoProvince: '',
        QuoDistricts: '',
        QuoSubDistricts: '',
        QuoZipCode: '',
        QuoStatus: 0,
        QuoRemark: '',
        QuoShippingPrice: 0
        /*        QuoTaxID:''*/

    };


    $scope.ListSizes = [];
    $scope.ListColors = [];

    // เอาไว้ Copy ข้อมูลมาแสดงใน Table
    $scope.Entries = [];

    $scope.TotalSum = 0;

    //ng - model="NewEntry.SelectedSize" 

    // New Entry Model
    $scope.NewEntry = {
        SelectedStyleName: '',
        SelectedSku: '',
        SelectedSize: '',
        SelectedColor: '',
        Quantity: 0,
        PricePerUnit: 0,
        TotalPrice: 0
    };


    $http.post('/Home/CheckUser')
        .then(function (response) {
            $scope.userData = response.data; // เก็บข้อมูลที่ดึงมาในตัวแปร 

            console.log(response.data);
        });

    $scope.files = [];
    $scope.CreateQuo = function () {
        // Redirect to CreateQuo View
        window.location.href = '/Home/CreateQuo';
    };

    /*EXEC gnerateQuotationNumber*/
    $scope.GetSku = function () {
        $http.get('/Home/GetSku')
            .then(function (response) {
                $scope.ListDropSku = response.data;
                console.log("Sku Number:", response.data);
            });
        //$scope.getUserData();
        //$scope.GetOrderNos();

    };
    /*GetSkuCode(QuoData.StyleName)*/
    $scope.GetSkuCode = function (styleName) {
        console.log("Selected StyleName:", styleName);
        $http.post('/Home/GetSkuCode', {
            StyleCode: styleName
        })
            .then(function (response) {
                $scope.skuCode = response.data;
                console.log("Sku Codes:", response.data);
            })
            .catch(function (error) {
                console.error("Error fetching SKU Codes:", error);
            });
    };

    /* ListCompany*/

    /* $scope.GetPageLoad()*/ //เรียกใช้ฟังก์ชัน - > มีการเรียกจากหน้า ng-init="GetPageLoad()"
    $scope.GetColors = function () {
        $http.get('/Home/GetColors')
            .then(function (response) {
                $scope.ListColors = response.data;
                console.log("Colors:", response.data);
            });
    };
    $scope.GetSizes = function () {
        $http.get('/Home/GetSizes')
            .then(function (response) {
                $scope.ListSizes = response.data;
                console.log("Colors:", response.data);
            });
    };

    $scope.GetOrderType = function () {


        $http.get('/Home/GetOrderType')
            .then(function (response) {
                $scope.ListTypeSell = response.data;
                console.log("OrderTypes:", response.data);
            });

    }

    $scope.AddEntry = function () {
        if (!$scope.skuCode || !$scope.QuoData.StyleName || !$scope.NewEntry.SelectedSize || !$scope.NewEntry.SelectedColor || !$scope.NewEntry.Quantity || !$scope.NewEntry.PricePerUnit) {


            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Please fill in all information completely!"

            });
            return;
        }
        $scope.NewEntry.SelectedStyleName = $scope.QuoData.StyleName;
        $scope.NewEntry.SelectedSku = $scope.skuCode;
        $scope.Entries.push(angular.copy($scope.NewEntry));
        //คำนวนใหม่
        $scope.CalculateTotalSum()
        $scope.CalculateQty(); // คำนวณยอดรวมของ Quantity
        $scope.NewEntry = {
            SelectedStyleName: '',
            SelectedSku: '',
            SelectedSize: '',
            SelectedColor: '',
            Quantity: 0,
            PricePerUnit: 0,
            TotalPrice: 0
        };
        //$scope.QuoData.StyleName = '';
        //$scope.skuCode = '';
    };

    $scope.RemoveEntry = function (index) {

        if (index >= 0 && index < $scope.Entries.length) {
            $scope.Entries.splice(index, 1);
            console.log("After removal:", $scope.Entries);
            //คำนวนใหม่
            $scope.CalculateTotalSum()
            $scope.CalculateQty(); // คำนวณยอดรวมของ Quantity
        } else {
            alert('555');
        }
    };

    $scope.TotalSum = 0;

    // ฟังก์ชันคำนวณยอดรวมทั้งหมด
    $scope.CalculateTotalSum = function () {

            //console.log("CalculateTotalSum Called");

            // เช็คว่าเป็น Array - ตั้งต้นเป็น Array ว่าง (ต้องเช็คเพราะว่าบางทีมันมองเป็น NULL)
            const entries = Array.isArray($scope.Entries) ? $scope.Entries : [];

            //console.log("Entries:", entries);

            // Entries
            const entryTotal = entries.reduce(function (sum, entry) {
                return sum + (entry.Quantity * entry.PricePerUnit);
            }, 0);

            //console.log("Entry Total:", entryTotal);

            // เช็ค แปลง QuoShippingPrice เป็น Number
            const shippingPrice = isNaN(parseFloat($scope.QuoData.QuoShippingPrice)) ? 0 : parseFloat($scope.QuoData.QuoShippingPrice);
            //console.log("Parsed Shipping Price:", shippingPrice);

            // Sum and update TotalPrice
            $scope.QuoData.TotalPrice = entryTotal + shippingPrice;
            /*console.log("Total Price Updated:", $scope.QuoData.TotalPrice);*/
       
    };

    $scope.CalculateQty = function () {
        $scope.QuoData.TotalQty = $scope.Entries.reduce(function (sum, entry) {
            return sum + entry.Quantity;
        }, 0);
    };



    $scope.QuoData.TotalPrice = $scope.TotalSum
    $scope.NewQuoNumber = [];

    //GenQuotationNumber
    $scope.GenerateQuotationNumber = function () {
        $http.get('/Home/GenerateQuotationNumber')
            .then(function (response) {
                $scope.NewQuoNumber = response.data;
                console.log("QuotationNumber:", response.data);
            });

    }

    $scope.SaveQuotation = function (QuoData, SelectedProvinces, SelectedDistricts,
        SelectedSub, SZipcode, skuCode, SelectedTypeSell, Entries) {
        // Validate ค่าว่าง
        if (!SelectedTypeSell || SelectedTypeSell.trim() === "") {
            Swal.fire({
                icon: "error",
                title: "Validation Error",
                text: "กรุณาระบุข้อมูล Quotation Type"
            });
            return;
        }

        if ((!QuoData.CustomerName || QuoData.CustomerName.trim() === "") &&
            (!QuoData.CompanyName || QuoData.CompanyName.trim() === "")) {
            Swal.fire({
                icon: "error",
                title: "Validation Error",
                text: "กรุณาระบุข้อมูล Customer Name หรือ Company Name"
            });
            return;
        }

        if (!Entries || Entries.length === 0) {
            Swal.fire({
                icon: "error",
                title: "Validation Error",
                text: "โปรดระบุสินค้า อย่างน้อย 1 รายการ"
            });
            return;
        }

        $http.post('/Home/SaveQuotation', {
            QuotationNumber: QuoData.QuoNumber,
            CustomerName: QuoData.CustomerName,
            OrderDate: QuoData.OrderDate,
            OrderStatus: "",
            ShipDate: QuoData.ShipDate,
            TotalQty: QuoData.TotalQty,
            TotalPrice: QuoData.TotalPrice,
            CustomerEmail: QuoData.CustomerEmail,
            CustomerAddress: QuoData.CustomerAddress,
            CustomerPhone: QuoData.CustomerPhone,
            Remark: '',
            CreateBy: QuoData.CreateBy,
            CreateDate: QuoData.CreateDate,
            /*  CustomerAddressTax: dataQuotation.CustomerAddressTax,*/
            QuoProvince: SelectedProvinces,
            QuoStatus: 0,
            QuoDistricts: SelectedDistricts,
            QuoSubDistricts: SelectedSub,
            QuoZipCode: SZipcode,
            QuoCompanyName: QuoData.CompanyName,
            QuoRemark: QuoData.Remark,
            QuoLastname: QuoData.QuoLastname,
            QuoTaxID: QuoData.QuoTaxID,
            QuoType: SelectedTypeSell,
            QuoShippingPrice: QuoData.QuoShippingPrice
        }).then(function (response) {
    
            var generatedQuotationNumber = response.data.quotationNumber;

            var updatedEntries = angular.copy(Entries);
            updatedEntries.forEach(function (entry) {
                entry.QuotationNumber = generatedQuotationNumber;
            });

            $http.post('/Home/SaveToProductTable', updatedEntries)
                .then(function (response) {
                    console.log(response.data);
                    var QuoNumber = response.data[0].quotationNumber;
                    console.log(QuoNumber);

                    // Update Quotation station-----*

                    Swal.fire({
                        icon: "success",
                        title: "Save Complete",
                        text: "New Quotation : " + QuoNumber + " Created."
                    }).then(function () {
                        // Redirect ไปหน้า index 
                        $window.location.href = "/Home/Index";
                    });
                })
                .catch(function (error) {
                    console.error("Error while saving entries:", error);
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: "Failed to save entries."
                    });
                });
           // Add Update Quo Status
        }).catch(function (error) {
            console.error("Error:", error);
            alert("ไม่สามารถบันทึกข้อมูลได้");
        });
    };


    $scope.PrintPDF = function (NumberQuo) {
        // รับค่า QuotationNumber จากข้อมูลที่กรอกโดยผู้ใช้
        console.log(NumberQuo);
        var quotationNumber = NumberQuo;
        /* var quotationNumber = $scope.QuoData.QuoNumber;*/

        // ตรวจสอบว่ามี QuotationNumber
        if (!quotationNumber) {
            alert("กรุณากรอกหมายเลขใบเสนอราคา");
            return;
        }

        // เรียกใช้ API เพื่อสร้าง PDF
        var url = '/PDF/PrintPDF?quotationNumber=' + encodeURIComponent(quotationNumber);

        // เปิดแท็บใหม่เพื่อดาวน์โหลดหรือแสดงไฟล์ PDF
        window.open(url, '_blank');
    };



    $scope.ValidateTaxID = function () {
        //$scope.QuoData.CustomerTaxID = $scope.QuoData.CustomerTaxID.replace(/\D/g, ''); // ลบตัวอักษรที่ไม่ใช่ตัวเลข
        //if ($scope.QuoData.CustomerTaxID.length > 13) {
        //    $scope.QuoData.CustomerTaxID = $scope.QuoData.CustomerTaxID.slice(0, 13); // จำกัดความยาวไม่เกิน 13 หลัก
        //}
    };


    $scope.GetPageLoad = function () {
        $scope.GetSku() // โหลด Style Name
        $scope.GetColors() // โหลด Color
        $scope.GetSizes();     // โหลด Sizes
        $scope.GetProvince() // โหลด จังหวัด
        $scope.GetOrderType(); // โหลด Type order
        $scope.GetLoadRemark() // โหลด Master remark
        

    };


    $scope.GetProvince = function () {
        $http.get('/Home/GetProvinces')
            .then(function (response) {
                $scope.ListProvinces = response.data;
            });
    }

    $scope.GetListDistricts = function (Provincess) {

        // ส่งข้อมูลไปยัง Backend
        $http.post('/Home/GetDistricts',
            {
                Provinces: Provincess
            })
            .then(function (response) {
                $scope.ListDistricts = response.data;
            })
            .catch(function (error) {
                console.error("Error:", error);
            });
    }

    $scope.GetListSub = function (SelectedDistricts, SelectedProvinces) {
        //Get SubDist Where จังหวัด, อำเภอ
        console.log(SelectedDistricts, SelectedProvinces);
        // ส่งข้อมูลไปยัง Backend
        $http.post('/Home/GetListSubs',
            {
                Districts: SelectedDistricts,
                Provinces: SelectedProvinces
            })
            .then(function (response) {
                console.log("Response จาก Backend:", response);
                $scope.ListSub = response.data; // เก็บผลลัพธ์จาก Backend
                console.log("ListDistricts:", $scope.ListSub);
            })
            .catch(function (error) {
                console.error("Error:", error);
            });
    }

    $scope.GetListZipcode = function (SelectedSub, SelectedDistricts) {

        console.log(SelectedSub, SelectedDistricts);
        // ส่งข้อมูลไปยัง Backend
        $http.post('/Home/GetListZipcode',
            {

                Districts: SelectedDistricts,
                SubDistricts: SelectedSub
            })
            .then(function (response) {
                $scope.SZipcode = response.data; // เก็บผลลัพธ์จาก Backend

                console.log($scope.SZipcode);
            })
            .catch(function (error) {
                console.error("Error:", error);
            });
    }

    //หน้า main กำหนด  ng-init="Getdataindex()"
    $scope.Getdataindex = function () {

        $http.post('/Home/GetdataQuo')
            .then(function (response) {
                $scope.ListQuo = response.data; // เก็บผลลัพธ์จาก Backend
                console.log($scope.ListQuo);

            })
            .catch(function (error) {
                console.error("Error:", error);
            });
    }

    // ฟังก์ชัน Edit เพื่อเปลี่ยนเส้นทางไปยังหน้าที่ต้องการแก้ไข
    $scope.editQuotation = function (quotationNumber) {
        // ไปยังหน้าแก้ไขข้อมูล โดยใช้ quotationNumber เป็นพารามิเตอร์

        $window.location.href = '/Home/EditQuo?quotationNumber=' + quotationNumber;
    }

    //$scope.GetdataQuoForEdit = function (searchQuo) {
    //    console.log(searchQuo);

    //    // ส่งข้อมูลไปยัง Backend
    //    $http.post('/Home/GetdataQuoForEdit', {
    //        QuotationNumber: searchQuo
    //    })
    //        .then(function (response) {
    //            $scope.ListDistricts = response.data;
    //        })
    //        .catch(function (error) {
    //            console.error("Error:", error);
    //        });



    //}

    function formatDateToDDMMYYYY(date) {
        const day = String(date.getDate()).padStart(2, '0'); // เติม 0 ด้านหน้าให้เป็น 2 หลัก
        const month = String(date.getMonth() + 1).padStart(2, '0'); // เดือนเริ่มจาก 0 จึงต้อง +1
        const year = date.getFullYear();
        return `${day}/${month}/${year}`; // รูปแบบ dd/mm/yyyy
    }


    $scope.GetdataQuoForEdit = function (searchQuo) {
        console.log(searchQuo);
        $scope.GetPageLoad();

        $http.post('/Home/GetdataQuoForEdit', {
            QuotationNumber: searchQuo
        }).then(function (response) {

            const shipDate = response.data.shipDate
                ? formatDateToDDMMYYYY(new Date(response.data.shipDate))
                : '';
            /* $scope.SelectedProvinces = response.data.provinces;*/

            $scope.QuoData = {
                QuoNumber: response.data.quotationNumber,
                CustomerName: response.data.customerName,
                /* CustFirstname: response.data.custFirstname,*/
                /* CustLastname: response.data.custLastname,*/
                CompanyName: response.data.quoCompanyName,
                OrderDate: new Date().toISOString().slice(0, 10), // รูปแบบ yyyy-MM-dd
                OrderStatus: '', //กำหนดให้เป็น Processing
               
                TotalQty: response.data.totalQty,
                TotalPrice: response.data.totalPrice,
                CustomerEmail: response.data.customerEmail,
                CustomerAddress: response.data.customerAddress,
                CustomerTaxID: response.data.customerTaxID,
                CustomerAddressTax: response.data.customerAddressTax,
                CustomerPhone: response.data.customerPhone,
                /* Remark: response.data.remark,*/
                CreateBy: 'ADMIN',// ***
                CreateDate: new Date().toISOString().slice(0, 10),// รูปแบบ dd/MM/yyyy HH:mm:ss
                StyleName: response.data.styleName,// ไม่มี MAP    
                QuoType: response.data.quoType,
                QuoLastname: response.data.quoLastname,
                // QuoCompanyName: response.data.quoCompanyName,
                //QuoProvince: response.data.quoProvince,
                //QuoDistricts: response.data.quoDistricts,
                //QuoSubDistricts: response.data.quoSubDistricts,
                //QuoZipCode: response.data.quoZipCode,
               
                Remark: response.data.quoRemark,
                QuoTaxID: response.data.quoTaxID,
                QuoShippingPrice: response.data.quoShippingPrice,
                QuoStatus: response.data.quoStatus, // เพิ่ม QuoStatus
                ShipDate: shipDate  // เพิ่ม ShipDate
                

            };



            $scope.isConfirmed = $scope.QuoData.QuoStatus.toString() === '1';
            $scope.selectedShipDate = $scope.isConfirmed ? $scope.QuoData.ShipDate : '';

        



            $scope.SelectedTypeSell = $scope.QuoData.QuoType;
            console.log($scope.QuoData.QuoType);

            $scope.SelectedProvinces = response.data.quoProvince;

            $scope.GetListDistricts(response.data.quoProvince);



            $scope.SelectedDistricts = response.data.quoDistricts;
            console.log($scope.SelectedDistricts);


            $scope.GetListSub(response.data.quoDistricts, response.data.quoProvince)

            $scope.SelectedSub = response.data.quoSubDistricts;



            $scope.SZipcode = response.data.quoZipCode;

            /*console.log("TEST" + $scope.SelectedProvinces)*/

            $http.post('/Home/GetForEditProduct', {
                QuotationNumber: searchQuo
            }).then(function (response) {
                console.log(response.data);

                // ตรวจสอบว่า response.data เป็น Array หรือไม่
                if (Array.isArray(response.data)) {
                    $scope.Entries = response.data.map(entry => ({
                        SelectedStyleName: entry.productName,
                        SelectedSku: entry.skuCodeFull,
                        SelectedSize: entry.size,
                        SelectedColor: entry.color,
                        Quantity: entry.qty,
                        PricePerUnit: entry.price,
                        TotalPrice: entry.qty * entry.price
                    }));
                } else {
                    console.error("Unexpected data format:", response.data);
                    $scope.Entries = [];
                }

                $scope.CalculateTotalSum();
                $scope.CalculateQty();
            });
            $scope.GetLoadFiles(); //Load file

        })
       
    }
            
    // Function Update

    $scope.UpdateQuotation = function () {
        console.log("Update");
        //!$scope.QuoData.CustomerName 
        // Validate before sending
        if (!$scope.QuoData.QuoNumber || !$scope.Entries.length) {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Please fill in all required fields!"
            });
            return;
        }

        //$scope.isConfirmed = false; // สถานะ Checkbox เริ่มต้น
      /*  $scope.selectedShipDate = "";*/ // วันที่จัดส่งเริ่มต้น
        var quoStatus = $scope.isConfirmed ? 1 : 0; // 1 for confirmed, 0 otherwise
      /*  var shipDate = $scope.isConfirmed ? $scope.selectedShipDate : new Date().toISOString().slice(0, 10);*/ // Use selected date or default to today
        var shipDate = $scope.isConfirmed
            ? new Date($scope.selectedShipDate.split('/').reverse().join('-')) // แปลงจาก dd/mm/yyyy เป็น Date Object
            : new Date(); // กรณีไม่ได้เลือกวันใช้วันปัจจุบัน

        //console.log(shipDate);

        //$scope.formattedDate = $filter('date')($scope.selectedShipDate, 'dd/MM/yyyy HH:mm:ss');


        console.log($scope.formattedDate);
        //// Prepare data for update
        //console.log("ShipDate " + shipDate);
        //console.log("Quo status " + quoStatus);
        const updateData = {
            QuotationNumber: $scope.QuoData.QuoNumber,
            QuoType: $scope.SelectedTypeSell,
            CustomerName: $scope.QuoData.CustomerName,
            QuoLastname: $scope.QuoData.QuoLastname,
            QuoCompanyName: $scope.QuoData.CompanyName,
            OrderDate: $scope.QuoData.OrderDate,
            ShipDate: shipDate.toISOString(), 
            TotalQty: $scope.QuoData.TotalQty,
            TotalPrice: $scope.QuoData.TotalPrice,
            Remark: $scope.QuoData.Remark,
            CustomerAddress: $scope.QuoData.CustomerAddress,
            CustomerPhone: $scope.QuoData.CustomerPhone,
            QuoTaxID: $scope.QuoData.QuoTaxID,
            CustomerEmail: $scope.QuoData.CustomerEmail,
            QuoProvince: $scope.SelectedProvinces,
            QuoDistricts: $scope.SelectedDistricts,
            QuoSubDistricts: $scope.SelectedSub,
            QuoZipCode: $scope.SZipcode,
            QuoShippingPrice: $scope.QuoData.QuoShippingPrice,
            QuoStatus: quoStatus,
            Entries: $scope.Entries.map(entry => ({       
                ProductName: entry.SelectedStyleName,
                SKUCodeFull: entry.SelectedSku,
                Sku: entry.SelectedSku, // แก้ไขให้ส่งค่าที่ถูกต้อง
                Qty: entry.Quantity,
                Size: entry.SelectedSize,
                Color: entry.SelectedColor,
                Price: entry.PricePerUnit
            }))
        };
        console.log("updateData" + updateData.QuoStatus);
        console.log("updateData" + updateData.ShipDate);

        // Send update request
        $http.post('/Home/UpdateQuotation', updateData)
            .then(function (response) {
                Swal.fire({
                    icon: "success",
                    title: "Update Complete",
                    text: "Quotation updated successfully."
                }).then(function () {
                    $window.location.href = "/Home/Index";
                });
            })
            .catch(function (error) {
                console.error("Update Error:", error);
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Failed to update quotation."
                });
            });
    };

    
    $scope.Cancel = function () {
        Swal.fire({
            title: 'Are you sure?',
            text: "Changes will not be saved!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, cancel!',
            cancelButtonText: 'No, stay here'
        }).then((result) => {
            if (result.isConfirmed) {
                $window.location.href = "/Home/Index";
            }
        });
    };




    $scope.GetLoadRemark = function () {
        $http.get('/Home/GetLoadRemark')
            .then(function (response) {
                $scope.Remarks = response.data; // เก็บข้อมูล Remark ใน Scope
            }, function (error) {
                console.error("Error loading remarks:", error);
            });
    };

    $scope.DeleteQuo = function (quoNumber) {
      

        $http.post('/Home/DeleteQuo', {
            QuotationNumber: quoNumber
        })
            .then(function (response) {
                console.log("Data : ", response.data);
                var QuoNumber = response.data.quotationNumber; 
                console.log("TEST : " + QuoNumber);
                Swal.fire({
                    icon: "success",
                    title: "Delete Complete",
                    text: "Quotation : " + QuoNumber + " Deleted."
                }).then(function () {
                    $window.location.href = "/Home/Index";
                });
            })
            .catch(function (error) {
                console.error("Delete Error:", error);
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Failed to delete quotation."
                });
            });
    }

    $scope.GetLoadOrderInfomation = function () {
        $http.get('')
            .then(function (response) {
               /* $scope.Remarks = response.data; // เก็บข้อมูล Remark ใน Scope*/
            }, function (error) {
                console.error("Error loading remarks:", error);
            });
    };

    $scope.GetPageQuotation = function () {
       
        $window.location.href = '/Home/Index';
    }
    $scope.GetPageOrderInformation = function () {

        $window.location.href = '/Home/OrderInformation';
    }

    // File

    $scope.UploadFile = function () {
        var fileInput = document.getElementById('fileInput').files[0];

        if (!fileInput) {
            Swal.fire({
                icon: "warning",
                title: "No file selected",
                text: "Please select a file before uploading."
            });
            return;
        }

        // ตรวจสอบและตั้งค่า fileDescription เป็นค่าว่างถ้าไม่มีการกรอก
        if (!$scope.fileDescription || $scope.fileDescription.trim() === "") {
            $scope.fileDescription = "";
        }

        var formData = new FormData();
        formData.append("file", fileInput);
        formData.append("fileDescription", $scope.fileDescription);
        formData.append("quotationNumber", $scope.QuoData.QuoNumber); // รับ QuotationNumber จาก Model

        $http.post('/Home/UploadFile', formData, {
            headers: { 'Content-Type': undefined }
        }).then(function (response) {
            // ใช้ $scope.$apply เพื่อกระตุ้น AngularJS ให้จับการเปลี่ยนแปลง
            $scope.$applyAsync(function () {
                $scope.files.push(response.data.data); // เพิ่มไฟล์ใหม่ลงในตาราง
            });

            Swal.fire({
                icon: "success",
                title: "File uploaded",
                text: "The file has been uploaded successfully."
            });


            $scope.fileDescription = ""; // ล้างคำอธิบาย
            document.getElementById('fileInput').value = null; // ล้างไฟล์ที่เลือก
        }).catch(function (error) {
            console.error("Error uploading file:", error);
            Swal.fire({
                icon: "error",
                title: "Upload Failed",
                text: "An error occurred while uploading the file."
            });
        });
    };


    $scope.downloadFile = function (filePath) {
        window.open('/Home/DownloadFile?filePath=' + encodeURIComponent(filePath), '_blank');
    };

    $scope.deleteFile = function (filePath, index) {
        Swal.fire({
            title: "Are you sure?",
            text: "This file will be permanently deleted!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "Cancel"
        }).then((result) => {
            if (result.isConfirmed) {
                $http.delete('/Home/DeleteFile', {
                    params: { filePath: filePath, quotationNumber: $scope.QuoData.QuoNumber }
                }).then(function (response) {
                    Swal.fire({
                        icon: "success",
                        title: "Deleted",
                        text: response.data.Message
                    });
                    $scope.files.splice(index, 1); // ลบไฟล์ออกจากตาราง
                }).catch(function (error) {
                    console.error("Error deleting file:", error);
                    Swal.fire({
                        icon: "error",
                        title: "Delete Failed",
                        text: "An error occurred while deleting the file."
                    });
                });
            }
        });
    };

    $scope.GetLoadFiles = function () {
        // ตรวจสอบว่ามี Quotation Number หรือไม่
        console.log($scope.QuoData.QuoNumber + " Quo")
        if (!$scope.QuoData.QuoNumber) {
            console.warn("Quotation number is required.");
            return;
        }

        // เรียก API ดึงข้อมูลไฟล์
        $http.get('/Home/GetQuotationFiles', {
            params: { quotationNumber: $scope.QuoData.QuoNumber }
           
        }).then(function (response) {
            $scope.files = response.data; // เก็บข้อมูลไฟล์ใน $scope.files
        }, function (error) {
            console.error("Error loading files:", error);
        });
    };


    $scope.updateShipmentDate = function () {
        if ($scope.isConfirmed) {
            // กรณี Confirm เป็นจริง ให้ตั้งค่า ShipDate
            $scope.selectedShipDate = $scope.QuoData.ShipDate || '';
        } else {
            // กรณีไม่ Confirm ให้ลบค่า ShipDate
            $scope.clearShipmentDate();
        }
    };

    $scope.clearShipmentDate = function () {
        $scope.selectedShipDate = ''; // เคลียร์ค่าของ ShipDate
    };

    $scope.$watch('isConfirmed', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.updateShipmentDate();
        }
    });

    $scope.viewQuotation = function (quotationNumber) {
    
        $window.location.href = '/Home/ViewPage?quotationNumber=' + quotationNumber;
      
    };

    $scope.deleteQuotation = function (quotationNumber) {
        Swal.fire({
            title: 'Are you sure?',
            text: 'Do you want to delete this Quotation?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!'
        }).then(function (result) {
            if (result.isConfirmed) {
                $http.post('/Home/DeleteQuo', { QuotationNumber: quotationNumber }).then(function () {
                    Swal.fire('Deleted!', 'Your Quotation has been deleted.', 'success');
                 /*   $scope.loadQuotations();*/ // โหลดข้อมูลใหม่
                }, function (error) {
                    Swal.fire('Error', 'Failed to delete quotation.', 'error');
                });
            }
        });
    };

    //$scope.loadQuotations = function () {
    //    $http.get('/Home/GetQuotations').then(function (response) {
    //        $scope.dataquo = response.data.data; // โหลด Quotation ใหม่
    //    });
    //};
    $scope.backHome = function () {
        $window.location.href = "/Home/Index"; 
    };


    // Order Information
    $scope.dataOrders = [];

    $scope.GetOrderInfo = function () {
        $http.get("/Home/GetOrderInfos")
            .then(function (response) {
                $scope.dataOrders = response.data.data; // Bind data to scope
                console.log($scope.dataOrders);
                initializeDataTable(); // เรียก DataTable หลังโหลดข้อมูล
            }, function (error) {
                console.error("Error fetching order information: ", error);
            });
    };

    //{
    //    data: 'shipDate',
    //        className: 'text-center',
    //            render: function (data) {
    //                return new Date(data).toLocaleDateString('th-TH');
    //            }
    //}
    function initializeDataTable() {
        $('#orderInfoTable').DataTable({
            destroy: true, // Reinitialize the table if needed
            data: $scope.dataOrders, // ใช้ข้อมูลจาก AngularJS
            columns: [
                { data: 'orderNumber', className: 'text-center' },
                {
                    data: 'orderDate',
                    className: 'text-center',
                    render: function (data) {
                        return new Date(data).toLocaleDateString('th-TH');
                    }
                },
                {
                    data: 'shipDate',
                    className: 'text-center',
                    render: function (data) {
                        return data ? new Date(data).toLocaleDateString('th-TH') : '-';
                    }
                },
                { data: 'totalQty', className: 'text-center' },
                {
                    data: 'orderStatus',
                    className: 'text-center'
                },
                {
                    data: null,
                    className: 'text-center',
                    render: function (data) {
                        return `<button class="btn btn-info view-button" data-order-number="${data.orderNumber}">View</button>`;
                    }
                },
                {
                    data: null,
                    className: 'text-center',
                    render: function (data) {
                        return `<button class="btn btn-success files-button" data-order-number="${data.orderNumber}">Files</button>`;
                    }
                }
            ],
            order: [[0, "desc"]], // เรียงลำดับ orderNumber จากมากไปน้อย
        });

        // Handle button clicks

        // Event handler for non-AngularJS buttons
        $('#orderInfoTable').on('click', '.view-button', function () {
            const orderNumber = $(this).data('order-number');
            $scope.openModal(orderNumber);
            $scope.GetProductOrder(orderNumber);
        });

        $('#orderInfoTable').on('click', '.files-button', function () {
            const orderNumber = $(this).data('order-number');
            $scope.Files(orderNumber);
        });
    }


    $scope.ViewOrderInfo = function (orderNumber) {
        window.location.href = `/Home/ViewOrder?orderNumber=${orderNumber}`;
    };

    $scope.Files = function (orderNumber) {
        window.location.href = `/Home/ViewAttachments?orderNumber=${orderNumber}`;
    };

    $scope.openModal = function (orderNumber) {
        // เรียกข้อมูลจากเซิร์ฟเวอร์เพื่อแสดงใน ModalPopup
        $http.get(`/Home/GetOrderDetails?orderNumber=${orderNumber}`)
            .then(function (response) {
                $scope.modalData = response.data; // เก็บข้อมูลใน $scope.modalData
                $('#OrderDetails').modal('show'); // เปิด ModalPopup
            }, function (error) {
                console.error("Error fetching order details: ", error);
            });

        //GetOrderDetails
    };

    $scope.GetProductOrder = function (orderNumber) {
        $http.get(`/Home/GetProductOrders?orderNumber=${orderNumber}`)
            .then(function (response) {
                $scope.productOrders = response.data; // เก็บข้อมูลใน scope
                console.log($scope.productOrders); // ตรวจสอบข้อมูลใน Console
            }, function (error) {
                console.error("Error fetching product orders: ", error);
            });
    };


    $scope.GetPageFile = function () {
        // ดึงค่าจาก URL Query String
        const urlParams = new URLSearchParams(window.location.search);
        const orderNumber = urlParams.get('orderNumber'); // ดึงค่าของ orderNumber
        $scope.orderNumber = orderNumber;
        if (!orderNumber) {
            console.error("Order number is missing in the URL.");
            return;
        }

        console.log("Order Number: ", orderNumber);

        $scope.GetDataQuoFileTable(orderNumber);
    };

    $scope.GetDataQuoFileTable = function (orderNumber) {


        //$http.get(`/Home/GetDataQuoFileTables`)
        //    .then(function (response) {
        //        $scope.quoFile = response.data; // เก็บข้อมูลใน scope
        //        console.log($scope.quoFile); // ตรวจสอบข้อมูลใน Console
        //    }, function (error) {
        //        console.error("Error QuoFile : ", error);
        //    });



        $http.get('/Home/GetDataQuoFileTables', { params: { orderNumber: orderNumber } })
            .then(function (response) {

                console.log("Loaded QuoFile Testttttttttt : ", response.data); 

                $scope.quoFile = response.data; // เก็บข้อมูลใน scope

                if (Array.isArray($scope.quoFile) && $scope.quoFile.length > 0) {
                    var quotationNumber = $scope.quoFile[0].quotationNumber;
                    $scope.quotationNumber = quotationNumber;
                } else {
                    $scope.quotationNumber = "";
                }



            
            })
            .catch(function (error) {
                console.error("Error loading QuoFile: ", error);
            });


        $http.get('/Home/GetDataOtherFileTable', { params: { orderNumber: orderNumber } })
            .then(function (response) {
                $scope.files = response.data; // เก็บข้อมูลใน scope
                console.log($scope.files); // ตรวจสอบข้อมูลใน Console
            }, function (error) {
                console.error("Error OtherFile : ", error);
            });
   

    };
   


    $scope.downloadQuotation = function (quotationNumber) {
        if (!quotationNumber) {
            Swal.fire({
                icon: "error",
                title: "Error",
                text: "Quotation Number is missing!"
            });
            return;
        }
        const url = '/PDF/PrintPDF?quotationNumber=' + encodeURIComponent(quotationNumber);
        window.open(url, '_blank');
    };

    // Upload File AboutOrder -ดูตารางที่ save file ของ Order
    $scope.UploadFileAboutOrder = function () {
        console.log("TEST FILE")
        var fileInput = document.getElementById("fileInput").files[0];
        if (!fileInput) {
            Swal.fire({
                icon: "warning",
                title: "No file selected",
                text: "Please select a file before uploading."
            });
            return;
        }

        // ตรวจสอบและตั้งค่า fileDescription เป็นค่าว่างถ้าไม่มีการกรอก
        if (!$scope.fileDescription || $scope.fileDescription.trim() === "") {
            $scope.fileDescription = "";
        }


        var formData = new FormData();
        formData.append("file", fileInput);
        formData.append("fileDescription", $scope.fileDescription || "");
        formData.append("orderNumber", $scope.orderNumber);

        $http.post("/Home/UploadFileAboutOrders", formData, {
            headers: { "Content-Type": undefined }
        }).then(function (response) {
            // ใช้ $scope.$apply เพื่อกระตุ้น AngularJS ให้จับการเปลี่ยนแปลง
            //console.log(response.data.sendFile + "show file all");
            //console.log("Response JSON:", JSON.stringify(response.data));

            $scope.files = $scope.files.map(file => {
                file.createdAt = new Date(file.createdAt);
                return file;
            });

            $scope.$applyAsync(function () {
             
                $scope.files.push(response.data.sendFile); // เพิ่มไฟล์ใหม่ลงในตาราง
            });

            Swal.fire({
                icon: "success",
                title: "File uploaded",
                text: "The file has been uploaded successfully."
            });


            $scope.fileDescription = ""; // ล้างคำอธิบาย
            document.getElementById('fileInput').value = null; // ล้างไฟล์ที่เลือก
        }).catch(function () {
            Swal.fire({
                icon: "error",
                title: "Upload Failed",
                text: "An error occurred while uploading the file."
            });
        });
    };

    // ********************
    //$scope.downloadFileAboutOrder = function (filePath) {
    //    window.open('/Home/DownloadFile?filePath=' + encodeURIComponent(filePath), '_blank');
    //};

    $scope.deleteFileAboutOrder = function (filePath, index) {
        Swal.fire({
            title: "Are you sure?",
            text: "This file will be permanently deleted!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "Cancel"
        }).then((result) => {
            if (result.isConfirmed) {
                $http.delete('/Home/DeleteFileAboutOrders', {
                    params: { filePath: filePath, orderNumber : $scope.orderNumber }
                }).then(function (response) {
                    Swal.fire({
                        icon: "success",
                        title: "Deleted",
                        text: response.data.Message
                    });
                    $scope.files.splice(index, 1); // ลบไฟล์ออกจากตาราง
                }).catch(function (error) {
                    console.error("Error deleting file:", error);
                    Swal.fire({
                        icon: "error",
                        title: "Delete Failed",
                        text: "An error occurred while deleting the file."
                    });
                });
            }
        });
    };



    $scope.backHomeOrder = function () {
        $window.location.href = "/Home/OrderInformation";
    };







});