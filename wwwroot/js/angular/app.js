
var app = angular.module('app', []);

app.controller('MyController', function ($scope, $http, $window) {
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
        CreateBy: 'jaruwan.s',
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
        // ส่งข้อมูลไปยัง Backend

        //$scope.SaveQuotation = function (QuoData, SelectedProvinces, SelectedDistricts,
        //    SelectedSub, SZipcode, skuCode, SelectedTypeSell, Entries) {
        //console.log(QuoData); 
        //console.log(skuCode);
        console.log(Entries);

        /*console.log(SelectedTypeSell.typeRecapFrom);*/

        $http.post('/Home/SaveQuotation', {
            QuotationNumber: QuoData.QuoNumber,
            CustomerName: QuoData.CustomerName,
            OrderDate: QuoData.OrderDate,
            OrderStatus: QuoData.OrderStatus,
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
            console.log("TYPE" + response.data);
            console.log(response.data.quotationNumber);
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
        console.log("editQuotation");
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



    $scope.GetdataQuoForEdit = function (searchQuo) {
        console.log(searchQuo);
        $scope.GetPageLoad();

        $http.post('/Home/GetdataQuoForEdit', {
            QuotationNumber: searchQuo
        }).then(function (response) {


            /* $scope.SelectedProvinces = response.data.provinces;*/

            $scope.QuoData = {
                QuoNumber: response.data.quotationNumber,
                CustomerName: response.data.customerName,
                /* CustFirstname: response.data.custFirstname,*/
                /* CustLastname: response.data.custLastname,*/
                CompanyName: response.data.quoCompanyName,
                OrderDate: new Date().toISOString().slice(0, 10), // รูปแบบ yyyy-MM-dd
                OrderStatus: 'Processing', //กำหนดให้เป็น Processing
                ShipDate: new Date(new Date().setDate(new Date().getDate() + 3)).toISOString().slice(0, 10), // วันที่ปัจจุบัน + 3 วันในรูปแบบ yyyy-MM-dd
                TotalQty: response.data.totalQty,
                TotalPrice: response.data.totalPrice,
                CustomerEmail: response.data.customerEmail,
                CustomerAddress: response.data.customerAddress,
                CustomerTaxID: response.data.customerTaxID,
                CustomerAddressTax: response.data.customerAddressTax,
                CustomerPhone: response.data.customerPhone,
                /* Remark: response.data.remark,*/
                CreateBy: 'jaruwan.s',// ***
                CreateDate: new Date().toISOString().slice(0, 10),// รูปแบบ dd/MM/yyyy HH:mm:ss
                StyleName: response.data.styleName,// ไม่มี MAP    
                QuoType: response.data.quoType,
                QuoLastname: response.data.quoLastname,
                // QuoCompanyName: response.data.quoCompanyName,
                //QuoProvince: response.data.quoProvince,
                //QuoDistricts: response.data.quoDistricts,
                //QuoSubDistricts: response.data.quoSubDistricts,
                //QuoZipCode: response.data.quoZipCode,
                QuoStatus: response.data.quoStatus,
                Remark: response.data.quoRemark,
                QuoTaxID: response.data.quoTaxID,
                QuoShippingPrice: response.data.quoShippingPrice

            };

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

        // Prepare data for update
        const updateData = {
            QuotationNumber: $scope.QuoData.QuoNumber,
            QuoType: $scope.SelectedTypeSell,
            CustomerName: $scope.QuoData.CustomerName,
            QuoLastname: $scope.QuoData.QuoLastname,
            QuoCompanyName: $scope.QuoData.CompanyName,
            OrderDate: $scope.QuoData.OrderDate,
            ShipDate: $scope.QuoData.ShipDate,
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


});