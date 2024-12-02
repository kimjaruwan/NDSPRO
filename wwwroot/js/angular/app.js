
var app = angular.module('app', []);

app.controller('MyController', function ($scope, $http) {
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
        QuoRemark: ''
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
        $scope.QuoData.TotalPrice = $scope.Entries.reduce(function (sum, entry) {
            return sum + (entry.Quantity * entry.PricePerUnit);
        }, 0);
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
            Remark: skuCode,
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
            QuoType: SelectedTypeSell.typeRecapFrom
        }).then(function (response) {
            console.log(response.data);
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

    $scope.GetListZipcode = function (SelectedSub, SelectedDistricts,) {

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

});
