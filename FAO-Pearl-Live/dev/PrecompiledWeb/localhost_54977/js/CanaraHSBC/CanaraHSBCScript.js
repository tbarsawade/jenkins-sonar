function DocumentCanaraHSBCController() {
    var constructor = {
        Alert: function () {
            // ===== for Canerahsbc - Other Expenses child item ===== Start
            var grid = document.getElementById("ContentPlaceHolder1_GRD28149");
            $("#ContentPlaceHolder1_GRD28149 td:first-child input[type=text]").bind("change", function (e) {
                CalculateExpense($(this), "expenseDate");
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28149 td:nth-child(2) select").bind("change", function (e) {
                CalculateExpense($(this), "expenseType");
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28149 td:nth-child(3) input[type=text]").bind("change", function (e) {
                CalculateExpense($(this), "claimAmt");
                e.preventDefault();
                return false;
            });
            // ===== for Canerahsbc - Other Expenses child item ===== End

            //// ===== for Canerahsbc - Lodging Expense Claim child item ===== start
            var grid = document.getElementById("ContentPlaceHolder1_GRD28147");
            $("#ContentPlaceHolder1_GRD28147 td:first-child select").bind("change", function (e) {
                //alert("Place of Stay");
                CalculateLodgingExpense();
            });
            $("#ContentPlaceHolder1_GRD28147 td:nth-child(2) select").bind("change", function (e) {
                //alert("Stay Type");
                CalculateLodgingExpense();
            });
            $("#ContentPlaceHolder1_GRD28147 td:nth-child(3) input[type=text]").bind("change", function (e) {
                //alert("from date");
                CalculateLodgingExpense();
            });
            $("#ContentPlaceHolder1_GRD28147 td:nth-child(4) input[type=text]").bind("change", function (e) {
                //alert("to date");
                CalculateLodgingExpense();
            });
            $("#ContentPlaceHolder1_GRD28147 td:nth-child(5) input[type=text]").bind("change", function (e) {
                //alert("claim amount");
                CalculateLodgingExpense();
            });
            //// ===== for Canerahsbc - Lodging Expense Claim child item ===== End

            //Canera Duplicate claims =========================================
            $("#ContentPlaceHolder1_GRD28147 td:first-child input[type=text]").bind("change", function (e) {
                CheckDuplicatefordate();
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28147 td:nth-child(2) select").bind("change", function (e) {
                //   CheckDuplicatefordateDB($(this));
                CheckDuplicatefordate($(this));

                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28147 td:nth-child(3) input[type=text]").bind("change", function (e) {
                // CheckDuplicatefordateDB($(this));
                CheckDuplicatefordate($(this));

                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28147 td:nth-child(4) input[type=text]").bind("change", function (e) {
                //  CheckDuplicatefordateDB($(this));
                CheckDuplicatefordate($(this));

                e.preventDefault();
                return false;
            });
            //Canera Duplicate claims =========================================

            // canera duplicate bill no. - mobile datacard
            $("#ContentPlaceHolder1_GRD28149 td:nth-child(4) input[type=text]").bind("change", function (e) {
                checkDuplicateBillNo($(this));
                e.preventDefault();
                return false;
            });

            //Canera Duplicate claims =========================================
            $("#ContentPlaceHolder1_GRD28146 td:first-child select").bind("change", function (e) {
                CheckDuplicatedateFortravel($(this));
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28146 td:nth-child(2) select").bind("change", function (e) {
                CheckDuplicatedateFortravel($(this));
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28146 td:nth-child(3) input[type=text]").bind("change", function (e) {
                CheckDuplicatedateFortravel($(this));
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28146 td:nth-child(4) input[type=text]").bind("change", function (e) {
                CheckDuplicatedateFortravel($(this));
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28146 td:nth-child(7) input[type=text]").bind("change", function (e) {
                checkBillNofor_trvl_Conv_Lodging($(this), 'Travel Expense Claim');
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28147 td:nth-child(6) input[type=text]").bind("change", function (e) {

                checkBillNofor_trvl_Conv_Lodging($(this), 'Lodging Expense Claim');
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD28148 td:nth-child(7) input[type=text]").bind("change", function (e) {
                checkBillNofor_trvl_Conv_Lodging($(this), 'Conveyance Expense Claim');
                e.preventDefault();
                return false;
            });

            // for canerahsbc 
            function CalculateExpense(value, text) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD28149");
                    var grdlenght = grd.rows.length;
                    var person = [], Business = [], valMobileOrDatacard = [];
                    for (var i = 1; i < (grdlenght - 1) ; i++) {
                        var objexpenseDate = grd.rows[i].cells[0].children[0];
                        var objexpenseType = grd.rows[i].cells[1].children[0];
                        var objclaimAmt = grd.rows[i].cells[2].children[0];
                        var expenseDate = objexpenseDate.value;
                        var expenseType = objexpenseType.value;
                        var claimAmt = objclaimAmt.value;
                        $(objclaimAmt).css('background-color', '');
                        if (expenseType == "1116055" || expenseType == "1113659") {
                            var arrexpenseDate = expenseDate.split("/");
                            var monthYear = arrexpenseDate[1] + "/" + arrexpenseDate[2];
                            var arr = [];
                            arr.push(monthYear);
                            arr.push(claimAmt);
                            arr.push(objclaimAmt);
                            person.push(arr);

			  var checkDate = new Date("2017-01-31");
                            var compDate = new Date('20' + arrexpenseDate[2]+'-'+ arrexpenseDate[1]+'-'+ arrexpenseDate[0]);
                            if (checkDate <compDate) {
                                $(objexpenseDate).css('background-color', 'red');
                            }
                            else {
                                $(objexpenseDate).css('background-color', '');
                            }


                            // new
                            var arr1 = [];

                            //arr1.push(monthYear)
                            //arr1.push(expenseType);
                            //valMobileOrDatacard.push(arr1);

                            //check duplication Mobile and Data card================
                            var expTypeText = "";
                            if (expenseType == "1116055") {
                                arr1 = [];
                                expTypeText = "Data Card Expenses";
                                arr1.push(monthYear)
                                arr1.push(expTypeText);
                                valMobileOrDatacard.push(arr1);
                            }
                            else {
                                arr1 = [];
                                expTypeText = "Mobile Expenses";
                                arr1.push(monthYear)
                                arr1.push(expTypeText);
                                valMobileOrDatacard.push(arr1);
                            }
                            var arrMobile = valMobileOrDatacard.filter(function (d) { return d[1] == expTypeText && d[0] == monthYear });
                            if (arrMobile.length < 2) {
                                var t = '{ documentType: "Other Expense Claim",EmpCode:"' + EmpCode + '",monthYear:"' + monthYear + '",expenseType: "' + expTypeText + '" }';
                                $.ajax({
                                    type: "POST",
                                    url: "/Documents.aspx/checkOtherExpClaimForManth",
                                    contentType: "application/json; charset=utf-8",
                                    data: t,
                                    dataType: "json",
                                    success: function (response) {
                                        var result = response.d;
                                        var ds = JSON.parse(result);
                                        if (ds.length > 0) {
                                            alert(expTypeText + " is already calimed for period  " + monthYear + "");
                                        }
                                    },
                                    error: function (data) {
                                        //Code For hiding preloader
                                    }
                                });
                            }
                            else {
                                alert(expTypeText + " is already calimed for period " + monthYear + "");
                                //valMobileOrDatacard = [];
                            }
                            // new
                        }
                        if (expenseType == "1113437") {
                            var arrexpenseDate = expenseDate.split("/");
                            var monthYear = arrexpenseDate[1] + "/" + arrexpenseDate[2];
                            var arr = [];
                            arr.push(monthYear);
                            arr.push(claimAmt);
                            arr.push(objclaimAmt);
                            Business.push(arr);
                        }
                    }
                    if (person.length > 0) {
                        var items = {}, base, key;
                        for (var i = 0; i < person.length; i++) {
                            base = person[i];
                            key = base[0];
                            if (!items[key]) {
                                items[key] = 0;
                            }
                            items[key] += parseInt(base[1]);
                        }

                        // Now, generate new array
                        var MobileoutputArr = [], temp;
                        for (key in items) {
                            temp = [key, items[key]]
                            MobileoutputArr.push(temp)
                        }

                        var t = '{ documentType: "Other Expense Claim",EmpCode:"' + EmpCode + '",ExpenseType:"Mobile" }';

                        //if (arrtxtobject.length > 0) {
                        //    for (var z = 0; z < arrtxtobject.length; z++) {
                        //        $(arrtxtobject[z][2]).css('background-color', '');
                        //    }
                        //}
                        $.ajax({
                            type: "POST",
                            url: "/Documents.aspx/GetdbSum",
                            contentType: "application/json; charset=utf-8",
                            data: t,
                            dataType: "json",
                            success: function (response) {
                                var result = response.d;
                                var ds = JSON.parse(result);

                                for (var x = 0; x < MobileoutputArr.length; x++) {
                                    var flage = 0;
                                    if (MobileoutputArr[x][1] > 1000) {
                                        arrtxtobject = $.grep(person, function (d) { return d[0] == MobileoutputArr[x][0]; })
                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                        }
                                        //alert("Mobile claim amount must be less than or equal to 1000.");
                                        flage = 1;
                                        //$(objclaimAmt).css('background-color', 'red');
                                        //break;
                                    }
                                    if (flage == 0) {
                                        for (var y = 0; y < ds.length; y++) {
                                            if (MobileoutputArr[x][0] == ds[y].ExpDate) {
                                                if ((MobileoutputArr[x][1] + ds[y].ClaimAmt) > 1000) {
                                                    //alert("Mobile claim amount must be less than or equal to 1000.");
                                                    arrtxtobject = $.grep(person, function (d) { return d[0] == MobileoutputArr[x][0]; })
                                                    for (var z = 0; z < arrtxtobject.length; z++) {
                                                        $(arrtxtobject[z][2]).css('background-color', 'red');
                                                    }
                                                    //alert("Mobile claim amount must be less than or equal to 1000.");
                                                    //break;
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            error: function (data) {
                                //Code For hiding preloader
                            }
                        });
                    }
                    if (Business.length > 0) {
                        var items = {}, base, key;
                        for (var i = 0; i < Business.length; i++) {
                            base = Business[i];
                            key = base[0];
                            if (!items[key]) {
                                items[key] = 0;
                            }
                            items[key] += parseInt(base[1]);
                        }

                        // Now, generate new array
                        var outputArr = [], temp;
                        for (key in items) {
                            temp = [key, items[key]]
                            outputArr.push(temp)
                        }

                        var t = '{ documentType: "Other Expense Claim",EmpCode:"' + EmpCode + '",ExpenseType:"Business" }';
                        $.ajax({
                            type: "POST",
                            url: "/Documents.aspx/GetdbSum",
                            contentType: "application/json; charset=utf-8",
                            data: t,
                            dataType: "json",
                            success: function (response) {
                                var result = response.d;
                                var ds = JSON.parse(result);
                                for (var x = 0; x < outputArr.length; x++) {
                                    var flage = 0;
                                    if (outputArr[x][1] > 1000) {
                                        //alert("Business claim amount must be less than or equal to 1000.")
                                        arrtxtobject = $.grep(Business, function (d) { return d[0] == outputArr[x][0]; })
                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                        }
                                        //alert("Mobile claim amount must be less than or equal to 1000.");
                                        flage = 1;
                                    }
                                    if (flage == 0) {
                                        for (var y = 0; y < ds.length; y++) {
                                            if (outputArr[x][0] == ds[y].ExpDate) {
                                                if ((outputArr[x][1] + ds[y].ClaimAmt) > 1000) {
                                                    //alert("Business claim amount must be less than or equal to 1000.");
                                                    arrtxtobject = $.grep(Business, function (d) { return d[0] == outputArr[x][0]; })
                                                    for (var z = 0; z < arrtxtobject.length; z++) {
                                                        $(arrtxtobject[z][2]).css('background-color', 'red');
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            error: function (data) {
                                //Code For hiding preloader
                            }
                        });
                    }
                    return false;
                }
                else {
                    alert("Please enter employee code.")
                }
            }

            // for canerahsbc 
            function CalculateLodgingExpense() {
                var grd = document.getElementById("ContentPlaceHolder1_GRD28147");
                var grdlenght = grd.rows.length;
                var person = [], Business = [];
                for (var i = 1; i < (grdlenght - 1) ; i++) {
                    var objPlaceOfStay = grd.rows[i].cells[0].children[0];
                    var objStayType = grd.rows[i].cells[1].children[0];
                    var objexpenseFDate = grd.rows[i].cells[2].children[0];
                    var objexpenseToDate = grd.rows[i].cells[3].children[0];
                    var objclaimAmt = grd.rows[i].cells[4].children[0];
                    //=========================================================
                    var PlaceOfStay = objPlaceOfStay.value;
                    var StayType = objStayType.value;
                    var expenseFDate = objexpenseFDate.value;
                    var expenseToDate = objexpenseToDate.value;
                    var claimAmt = objclaimAmt.value;
                    var diffDays = CalculateLodgingExpense_Canra_Day(expenseFDate, expenseToDate, StayType);
                    if (StayType == "1113489") {
                        if (PlaceOfStay == "1113477" || PlaceOfStay == "1113478" || PlaceOfStay == "1113479" ||
                            PlaceOfStay == "1113480" || PlaceOfStay == "1113481" || PlaceOfStay == "1113482" ||
                            PlaceOfStay == "1113483" || PlaceOfStay == "1113484" || PlaceOfStay == "1113485" ||
                            PlaceOfStay == "1113686" || PlaceOfStay == "1113689" || PlaceOfStay == "1113704" ||
                            PlaceOfStay == "1113708" || PlaceOfStay == "1113716" || PlaceOfStay == "1113723" ||
                            PlaceOfStay == "1113726" || PlaceOfStay == "1113730" || PlaceOfStay == "1113743" ||
                            PlaceOfStay == "1113752" || PlaceOfStay == "1113769" || PlaceOfStay == "1113794" ||
                            PlaceOfStay == "1113804" || PlaceOfStay == "1113805" || PlaceOfStay == "1113815" ||
                            PlaceOfStay == "1113908" || PlaceOfStay == "1114004" || PlaceOfStay == "1114009" ||
                            PlaceOfStay == "1114783" || PlaceOfStay == "1114864" || PlaceOfStay == "1114865" ||
                            PlaceOfStay == "1114866" || PlaceOfStay == "1114867") {

                            var CalClamAmount = (1700 * diffDays);
                            if (claimAmt > CalClamAmount) {
                                //   alert("Hotel clam amount less than " + CalClamAmount + " for metros citi.");
                                $(objclaimAmt).css('background-color', 'red');
                                //     break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                        }
                        else {
                            var CalClamAmount = (1200 * diffDays);
                            if (claimAmt > CalClamAmount) {
                                //      alert("Hotel clam amount less than " + CalClamAmount + " for other citi.");
                                $(objclaimAmt).css('background-color', 'red');
                                //     break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                        }
                    }
                    if (StayType == "1113656") {
                        if (PlaceOfStay == "1113477" || PlaceOfStay == "1113478" || PlaceOfStay == "1113479" ||
                            PlaceOfStay == "1113480" || PlaceOfStay == "1113481" || PlaceOfStay == "1113482" ||
                            PlaceOfStay == "1113483" || PlaceOfStay == "1113484" || PlaceOfStay == "1113485" ||
                            PlaceOfStay == "1113686" || PlaceOfStay == "1113689" || PlaceOfStay == "1113704" ||
                            PlaceOfStay == "1113708" || PlaceOfStay == "1113716" || PlaceOfStay == "1113723" ||
                            PlaceOfStay == "1113726" || PlaceOfStay == "1113730" || PlaceOfStay == "1113743" ||
                            PlaceOfStay == "1113752" || PlaceOfStay == "1113769" || PlaceOfStay == "1113794" ||
                            PlaceOfStay == "1113804" || PlaceOfStay == "1113805" || PlaceOfStay == "1113815" ||
                            PlaceOfStay == "1113908" || PlaceOfStay == "1114004" || PlaceOfStay == "1114009" ||
                            PlaceOfStay == "1114783" || PlaceOfStay == "1114864" || PlaceOfStay == "1114865" ||
                            PlaceOfStay == "1114866" || PlaceOfStay == "1114867") {

                            var CalClamAmount = (800 * diffDays);
                            if (claimAmt > CalClamAmount) {
                                //      alert("Food clam amount less than " + CalClamAmount + " for metros citi.");
                                $(objclaimAmt).css('background-color', 'red');
                                //     break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                        }
                        else {
                            var CalClamAmount = (600 * diffDays);
                            if (claimAmt > CalClamAmount) {
                                //   alert("Food clam amount less than " + CalClamAmount + " for other citi.");
                                $(objclaimAmt).css('background-color', 'red');
                                //    break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                        }
                    }
                    if (StayType == "1113490") {
                        if (PlaceOfStay == "1113477" || PlaceOfStay == "1113478" || PlaceOfStay == "1113479" ||
                            PlaceOfStay == "1113480" || PlaceOfStay == "1113481" || PlaceOfStay == "1113482" ||
                            PlaceOfStay == "1113483" || PlaceOfStay == "1113484" || PlaceOfStay == "1113485" ||
                            PlaceOfStay == "1113686" || PlaceOfStay == "1113689" || PlaceOfStay == "1113704" ||
                            PlaceOfStay == "1113708" || PlaceOfStay == "1113716" || PlaceOfStay == "1113723" ||
                            PlaceOfStay == "1113726" || PlaceOfStay == "1113730" || PlaceOfStay == "1113743" ||
                            PlaceOfStay == "1113752" || PlaceOfStay == "1113769" || PlaceOfStay == "1113794" ||
                            PlaceOfStay == "1113804" || PlaceOfStay == "1113805" || PlaceOfStay == "1113815" ||
                            PlaceOfStay == "1113908" || PlaceOfStay == "1114004" || PlaceOfStay == "1114009" ||
                            PlaceOfStay == "1114783" || PlaceOfStay == "1114864" || PlaceOfStay == "1114865" ||
                            PlaceOfStay == "1114866" || PlaceOfStay == "1114867") {

                            var CalClamAmount = (700 * diffDays);
                            if (claimAmt > CalClamAmount) {
                                //   alert("Hotel clam amount less than " + CalClamAmount + " for metros citi.");
                                $(objclaimAmt).css('background-color', 'red');
                                //     break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                        }
                        else {
                            var CalClamAmount = (500 * diffDays);
                            if (claimAmt > CalClamAmount) {
                                //      alert("Hotel clam amount less than " + CalClamAmount + " for other citi.");
                                $(objclaimAmt).css('background-color', 'red');
                                //     break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                        }
                    }
                    //=========================================================
                }
            }

            // for canerahsbc 
            function CalculateLodgingExpense_Canra_Day(FromDate, ToDate, stayType) {
                arrFromDate = FromDate.split('/');
                arrToDate = ToDate.split('/');
                var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
                var firstDate = new Date('20' + arrFromDate[2], arrFromDate[1], arrFromDate[0]);
                var secondDate = new Date('20' + arrToDate[2], arrToDate[1], arrToDate[0]);
                var diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
                if (stayType != 1113656) {
                    if (diffDays == 0)
                        diffDays = 1;
                }
                else {
                    if (diffDays == 0)
                        diffDays = 1;
                    else {
                        diffDays = diffDays + 1;
                    }
                }
                return diffDays
            }

            // for canerahsbc 
            function CalculateDay(FromDate, ToDate) {
                arrFromDate = FromDate.split('/');
                arrToDate = ToDate.split('/');
                var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
                var firstDate = new Date('20' + arrFromDate[2], arrFromDate[1], arrFromDate[0]);
                var secondDate = new Date('20' + arrToDate[2], arrToDate[1], arrToDate[0]);
                var diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
                if (diffDays == 0)
                    diffDays = 1;
                return diffDays
            }

            // Canera - Date duplicity check by Pallavi
            //To check Duplicate entries for Hotel/Own Arrangements in a particular period in Lodging Expense claim (canara)
            function CheckDuplicatefordate(value) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();

                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD28147");
                    var grdlenght = grd.rows.length;
                    var person = [], Business = [];
                    var isbreak = 0;
                    for (var i = 1; i < (grdlenght - 1) ; i++) {
                        if (isbreak != 0)
                        { break; }
                        var FrmDatestr = grd.rows[i].cells[2].children[0].value;
                        var ToDatestr = grd.rows[i].cells[3].children[0].value;
                        var frmarr = FrmDatestr.split('/');
                        var FrmDate = new Date('20' + frmarr[2], frmarr[1], frmarr[0]);
                        var Toarr = ToDatestr.split('/');
                        var ToDate = new Date('20' + Toarr[2], Toarr[1], Toarr[0]);
                        var StayType = grd.rows[i].cells[1].children[0].value;

                        if (FrmDate > ToDate) {
                            alert("To Date cannot be prior to From Date!");
                            $(value).css('background-color', 'red');
                            break;
                        }

                        if (StayType == "1113489" || StayType == "1113490")
                        { CheckDuplicatefordateDB(value); }
                        else if (StayType == "1113656")
                        { CheckDuplicatefordateDBFoodExpense(value); }
                        for (var j = 1; j < (grdlenght - 1) ; j++) {
                            var FrmDate2str = grd.rows[j].cells[2].children[0].value;
                            var ToDate2str = grd.rows[j].cells[3].children[0].value;
                            var frmarr2 = FrmDate2str.split('/');
                            var FrmDate2 = new Date('20' + frmarr2[2], frmarr2[1], frmarr2[0]);
                            var Toarr2 = ToDate2str.split('/');
                            var ToDate2 = new Date('20' + Toarr2[2], Toarr2[1], Toarr2[0]);
                            var StayType2 = grd.rows[j].cells[1].children[0].value;
                            $(grd.rows[j].cells[2].children[0]).css('background-color', '');
                            $(grd.rows[j].cells[3].children[0]).css('background-color', '');

                            if ((i < j) && (StayType == "1113489" || StayType == "1113490") && (StayType2 == "1113489" || StayType2 == "1113490")) {

                                if ((FrmDate2str == FrmDatestr) || (FrmDate2 > FrmDate && FrmDate2 < ToDate) || (FrmDate2str == ToDatestr)) {
                                    $(value).focus();
                                    alert("Already claimed for this period ");
                                    $(grd.rows[j].cells[2].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else {
                                    $(grd.rows[j].cells[2].children[0]).css('background-color', '');
                                }

                                if ((ToDate2str == FrmDatestr) || (ToDate2 > FrmDate && ToDate2 < ToDate) || (ToDate2str == ToDatestr)) {
                                    $(value).focus();
                                    alert("Already claimed for this period ");
                                    $(grd.rows[j].cells[3].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else { $(grd.rows[j].cells[3].children[0]).css('background-color', ''); }

                            }
                            else if ((i < j) && StayType == "1113656" && StayType2 == "1113656") {
                                if ((FrmDate2str == FrmDatestr) || (FrmDate2 > FrmDate && FrmDate2 < ToDate) || (FrmDate2str == ToDatestr)) {
                                    $(value).focus();
                                    alert("Already claimed for this period food expense nrml");
                                    $(grd.rows[j].cells[2].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else {
                                    $(grd.rows[j].cells[2].children[0]).css('background-color', '');
                                }

                                if ((ToDate2str == FrmDatestr) || (ToDate2 > FrmDate && ToDate2 < ToDate) || (ToDate2str == ToDatestr)) {
                                    $(value).focus();
                                    alert("Already claimed for this period food expense nrml ");
                                    $(grd.rows[j].cells[3].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else { $(grd.rows[j].cells[3].children[0]).css('background-color', ''); }
                            }


                        }
                    }


                    // CheckDuplicatefordateFoodExpense($(this))
                    return false;
                }
                else {
                    alert("Please enter employee code.")
                }

            }

            function CheckDuplicatefordateDB(txtdate) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
                var date = txtdate[0].value;

                if (EmpCode != "") {
                    var t = '{ EmpCode:"' + EmpCode + '",Date1:"' + date + '" }';
                    $.ajax({
                        type: "POST",
                        url: "/Documents.aspx/checkdateforDb",
                        contentType: "application/json; charset=utf-8",
                        data: t,
                        dataType: "json",
                        success: function (response) {
                            var result = response.d;
                            if (result > 0) {
                                alert("Already claimed for this period!");
                                $(txtdate).css('background-color', 'red');
                            }
                            else {

                                $(txtdate).css('background-color', '');
                            }
                        },
                        error: function (data) {
                            //Code For hiding preloader
                        }
                    });

                    return false;
                }
                else {
                    alert("Please enter employee code.")
                }

            }

            function CheckDuplicatefordateDBFoodExpense(txtdate) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
                var date = txtdate[0].value;

                if (EmpCode != "") {
                    var t = '{ EmpCode:"' + EmpCode + '",Date1:"' + date + '" }';
                    $.ajax({
                        type: "POST",
                        url: "/Documents.aspx/checkdateforDbFoodExpense",
                        contentType: "application/json; charset=utf-8",
                        data: t,
                        dataType: "json",
                        success: function (response) {
                            var result = response.d;
                            if (result > 0) {
                                alert("Already claimed for this period");
                                $(txtdate).css('background-color', 'red');
                            }
                            else {

                                $(txtdate).css('background-color', '');
                            }
                        },
                        error: function (data) {
                            //Code For hiding preloader
                        }
                    });

                    return false;
                }
                else {
                    alert("Please enter employee code.")
                }

            }

            function CheckDuplicatefordateFoodExpense(value) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();

                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD28147");
                    var grdlenght = grd.rows.length;
                    var person = [], Business = [];
                    var isbreak = 0;
                    for (var i = 1; i < (grdlenght - 1) ; i++) {
                        if (isbreak != 0)
                        { break; }
                        var FrmDatestr = grd.rows[i].cells[2].children[0].value;
                        var ToDatestr = grd.rows[i].cells[3].children[0].value;
                        var frmarr = FrmDatestr.split('/');
                        var FrmDate = new Date('20' + frmarr[2], frmarr[1], frmarr[0]);
                        var Toarr = ToDatestr.split('/');
                        var ToDate = new Date('20' + Toarr[2], Toarr[1], Toarr[0]);
                        var StayType = grd.rows[i].cells[1].children[0].value;
                        if (FrmDate > ToDate) {
                            alert("To Date cannot be prior to From Date!");
                            $(value).css('background-color', 'red');
                            break;
                        }

                        if (StayType == "1113656") {
                            CheckDuplicatefordateDBFoodExpense(value);
                        }

                        for (var j = 1; j < (grdlenght - 1) ; j++) {
                            var FrmDate2str = grd.rows[j].cells[2].children[0].value;
                            var ToDate2str = grd.rows[j].cells[3].children[0].value;
                            var frmarr2 = FrmDate2str.split('/');
                            var FrmDate2 = new Date('20' + frmarr2[2], frmarr2[1], frmarr2[0]);
                            var Toarr2 = ToDate2str.split('/');
                            var ToDate2 = new Date('20' + Toarr2[2], Toarr2[1], Toarr2[0]);
                            var StayType2 = grd.rows[j].cells[1].children[0].value;

                            $(grd.rows[j].cells[2].children[0]).css('background-color', '');
                            $(grd.rows[j].cells[3].children[0]).css('background-color', '');

                            if ((i < j) && (StayType == "1113656") && (StayType2 == "1113656")) {
                                if ((FrmDate2str == FrmDatestr) || (FrmDate2 > FrmDate && FrmDate2 < ToDate) || (FrmDate2str == ToDatestr)) {
                                    $(value).focus();
                                    alert("Already claimed for this period ");
                                    $(grd.rows[j].cells[2].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else {
                                    $(grd.rows[j].cells[2].children[0]).css('background-color', '');
                                }



                                if ((ToDate2str == FrmDatestr) || (ToDate2 > FrmDate && ToDate2 < ToDate) || (ToDate2str == ToDatestr)) {
                                    $(value).focus();
                                    alert("Already claimed for this period ");
                                    $(grd.rows[j].cells[3].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else {
                                    $(grd.rows[j].cells[3].children[0]).css('background-color', '');
                                }

                            }
                        }
                    }

                    return false;
                }
                else {
                    alert("Please enter employee code.")
                }

            }
            // Canera - Date duplicity check by Pallavi

            // canera billno. duplicate check
            function checkDuplicateBillNo(value) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
                $(value).css('background-color', '');
                CheckDuplicateBillNoDB(value);

                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD28149");
                    var grdlenght = grd.rows.length;

                    var isbreak = 0;
                    for (var i = 1; i < (grdlenght - 1) ; i++) {

                        var BillNo = grd.rows[i].cells[3].children[0].value;
                        var Type = grd.rows[i].cells[1].children[0].value;
                        if (isbreak != 0) {
                            break;
                        }

                        for (var j = 2; j < (grdlenght - 1) ; j++) {

                            var BillNo2 = grd.rows[j].cells[3].children[0].value;
                            var Type2 = grd.rows[i].cells[1].children[0].value;

                            if ((i < j) && BillNo2 != "") {

                                if (($.trim(BillNo) == $.trim(BillNo2)) && (Type == "1113659" || Type == "1116055") && (Type2 == "1113659" || Type2 == "1116055")) {
                                    $(value).focus();
                                    alert("Already claimed for this Bill No. ");
                                    $(grd.rows[j].cells[3].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else { $(grd.rows[j].cells[3].children[0]).css('background-color', ''); }


                            }



                        }
                    }


                    // CheckDuplicatefordateFoodExpense($(this))
                    return false;
                }
                else {
                    alert("Please enter employee code.")
                }

            }

   function CheckDuplicateBillNoDB(txtBillNo) {

                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
                var BillNo = txtBillNo[0].value;

                if (EmpCode != "") {
                    var t = '{ EmpCode:"' + EmpCode + '",BillNo:"' + BillNo + '" }';
                    $.ajax({
                        type: "POST",
                        url: "/Documents.aspx/checkBillNoforDb",
                        contentType: "application/json; charset=utf-8",
                        data: t,
                        dataType: "json",
                        success: function (response) {
                            var result = response.d;
                            var ds = JSON.parse(result);
                            if (ds[0].Count > 0) {
                                alert("Already claimed for this Bill No.");
                                $(txtBillNo).css('background-color', 'red');
                            }
                            else {

                                $(txtBillNo).css('background-color', '');
                            }
                        },
                        error: function (data) {
                            //Code For hiding preloader
                        }
                    });

                    return false;
                }
                else {
                    alert("Please enter employee code.");
                }

            }

            // canera billno. duplicate check

            //== canera - Travel expl. dates duplicity check 
            function checkBillNofor_trvl_Conv_Lodging(value, type) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
                // var BillNo = value[0].value;

                if (EmpCode != "") {

                    if (type == 'Travel Expense Claim') {
                        var grd = document.getElementById("ContentPlaceHolder1_GRD28146");
                        var grdlenght = grd.rows.length;

                        var isbreak = 0;
                        for (var i = 1; i < (grdlenght - 1) ; i++) {

                            var BillNo = grd.rows[i].cells[6].children[0].value;
                            var CompareTypeVal = grd.rows[i].cells[4].children[0].value;

                            var txtBillNo = grd.rows[i].cells[6].children[0];
                            if (CompareTypeVal == "0" && BillNo != "") {

                                $(grd.rows[i].cells[4].children[0]).css('background-color', 'red');
                                alert("Please select Mode Of Travel");
                                return;
                            }
                            else {
                                // $(grd.rows[i].cells[6].children[0]).html('');
                                $(grd.rows[i].cells[4].children[0]).css('background-color', '');
                            }
                            checkBillNofor_trvl_Conv_LodgingDB(EmpCode, BillNo, txtBillNo, 'Travel Expense Claim', CompareTypeVal);
                            if (isbreak != 0) {
                                break;
                            }
                            for (var j = 2; j < (grdlenght - 1) ; j++) {

                                var BillNo2 = grd.rows[j].cells[6].children[0].value;
                                var CompareTypeVal2 = grd.rows[j].cells[4].children[0].value;
                                if ((i < j) && BillNo2 != "") {

                                    if (($.trim(BillNo) == $.trim(BillNo2)) && CompareTypeVal == CompareTypeVal2) {
                                        $(value).focus();
                                        alert("Already claimed for this Bill No. ");
                                        $(grd.rows[j].cells[6].children[0]).css('background-color', 'red');
                                        isbreak = 1;
                                        break;
                                    }
                                    else { $(grd.rows[j].cells[6].children[0]).css('background-color', ''); }
                                }
                            }
                        }
                        return false;
                    }
                    else if (type == 'Lodging Expense Claim') {
                        var grd = document.getElementById("ContentPlaceHolder1_GRD28147");
                        var grdlenght = grd.rows.length;

                        var isbreak = 0;
                        for (var i = 1; i < (grdlenght - 1) ; i++) {

                            var BillNo = grd.rows[i].cells[5].children[0].value;
                            var CompareTypeVal = grd.rows[i].cells[1].children[0].value;

                            var txtBillNo = grd.rows[i].cells[5].children[0];
                            if (CompareTypeVal == "0" && BillNo != "") {
                                alert("Please select Stay Type");
                                $(grd.rows[i].cells[1].children[0]).css('background-color', 'red');
                                return;
                            }
                            else {
                                // $(txtBillNo).val('');
                                $(grd.rows[i].cells[1].children[0]).css('background-color', '');
                            }
                            checkBillNofor_trvl_Conv_LodgingDB(EmpCode, BillNo, txtBillNo, 'Lodging Expense Claim', CompareTypeVal);
                            if (isbreak != 0) {
                                break;
                            }
                            for (var j = 2; j < (grdlenght - 1) ; j++) {

                                var BillNo2 = grd.rows[j].cells[5].children[0].value;
                                var CompareTypeVal2 = grd.rows[j].cells[1].children[0].value;
                                if ((i < j) && BillNo2 != "") {

                                    if (($.trim(BillNo) == $.trim(BillNo2)) && CompareTypeVal == CompareTypeVal2) {
                                        $(value).focus();
                                        alert("Already claimed for this Bill No. ");
                                        $(grd.rows[j].cells[5].children[0]).css('background-color', 'red');
                                        isbreak = 1;
                                        break;
                                    }
                                    else { $(grd.rows[j].cells[5].children[0]).css('background-color', ''); }
                                }
                            }
                        }

                        return false;
                    }
                    else if (type == 'Conveyance Expense Claim') {
                        var grd = document.getElementById("ContentPlaceHolder1_GRD28148");
                        var grdlenght = grd.rows.length;

                        var isbreak = 0;
                        for (var i = 1; i < (grdlenght - 1) ; i++) {

                            var BillNo = grd.rows[i].cells[6].children[0].value;
                            var CompareTypeVal = grd.rows[i].cells[2].children[0].value;

                            var txtBillNo = grd.rows[i].cells[6].children[0];
                            if (CompareTypeVal == "0" && BillNo != "") {
                                alert("Please select Conveyance Type");
                                $(grd.rows[i].cells[2].children[0]).css('background-color', 'red');
                                return;
                            }
                            else {
                                // $(txtBillNo).val('');
                                $(grd.rows[i].cells[2].children[0]).css('background-color', '');
                            }
                            checkBillNofor_trvl_Conv_LodgingDB(EmpCode, BillNo, txtBillNo, 'Conveyance Expense Claim', CompareTypeVal);
                            if (isbreak != 0) {
                                break;
                            }
                            for (var j = 2; j < (grdlenght - 1) ; j++) {

                                var BillNo2 = grd.rows[j].cells[6].children[0].value;
                                var CompareTypeVal2 = grd.rows[j].cells[2].children[0].value;
                                if ((i < j) && BillNo2 != "") {

                                    if (($.trim(BillNo) == $.trim(BillNo2)) && CompareTypeVal == CompareTypeVal2) {
                                        $(value).focus();
                                        alert("Already claimed for this Bill No. ");
                                        $(grd.rows[j].cells[6].children[0]).css('background-color', 'red');
                                        isbreak = 1;
                                        break;
                                    }
                                    else { $(grd.rows[j].cells[6].children[0]).css('background-color', ''); }
                                }
                            }
                        }

                        return false;
                    }


                    return false;
                }
                else {
                    alert("Please enter employee code.");
                }
            }

            function checkBillNofor_trvl_Conv_LodgingDB(EmpCode, BillNo, value, type, CompareTypeVal) {
                var t = '{ EmpCode:"' + EmpCode + '",BillNo:"' + BillNo + '",Type:"' + type + '",CompareTypeVal:"' + CompareTypeVal + '" }';
                $(value).css('background-color', '');
                if (BillNo != "") {
                    $.ajax({

                        type: "POST",
                        url: "/Documents.aspx/checkBillNoforDb_trvl_Conv_Lodging",
                        contentType: "application/json; charset=utf-8",
                        data: t,
                        dataType: "json",
                        success: function (response) {
                            var result = response.d;
                            if (result > 0) {
                                alert("Already claimed for this Bill No. ");
                                $(value).css('background-color', 'red');
                            }

                            //else { $(value).css('background-color', ''); }
                        },
                        error: function (data) {
                            //Code For hiding preloader
                        }

                    });
                }
            }

            function CheckDuplicatedateFortravel(value) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
               
                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD28146");
                    var grdlenght = grd.rows.length;
                    var grdIndex = value[0].id.split("_");
                    var finalIndex = Number(grdIndex[grdIndex.length - 1]) + 1;
                    CheckDuplicatedateFortravelDB(value, grd.rows[finalIndex].cells[1].children[0].value, grd.rows[finalIndex].cells[0].children[0].value);
                    CheckDuplicatedateFortravelDBBackuEnd(grd.rows[finalIndex].cells[3].children[0], grd.rows[finalIndex].cells[2].children[0], grd.rows[finalIndex].cells[3].children[0].value, grd.rows[finalIndex].cells[2].children[0].value, grd.rows[finalIndex].cells[1].children[0].value, grd.rows[finalIndex].cells[0].children[0].value);
                    var isbreak = 0;
                    for (var i = 1; i < (grdlenght - 1) ; i++) {
                        if (isbreak != 0)
                        { break; }
                        var DeptCity = grd.rows[i].cells[0].children[0].value;
                        var Arrcity = grd.rows[i].cells[1].children[0].value;
                        var FrmDatestr = grd.rows[i].cells[2].children[0].value;
                        var ToDatestr = grd.rows[i].cells[3].children[0].value;
                        var frmarr = FrmDatestr.split('/');
                        var FrmDate = new Date('20' + frmarr[2], frmarr[1], frmarr[0]);
                        var Toarr = ToDatestr.split('/');
                        var ToDate = new Date('20' + Toarr[2], Toarr[1], Toarr[0]);
                        if (FrmDate > ToDate) {
                            alert("Arrival Date cannot be prior to Departure Date!");
                            $(value).css('background-color', 'red');
                            break;
                        }


                        for (var j = 1; j < (grdlenght - 1) ; j++) {
                            var DeptCity2 = grd.rows[j].cells[0].children[0].value;
                            var Arrcity2 = grd.rows[j].cells[1].children[0].value;
                            var FrmDate2str = grd.rows[j].cells[2].children[0].value;
                            var ToDate2str = grd.rows[j].cells[3].children[0].value;
                            var frmarr2 = FrmDate2str.split('/');
                            var FrmDate2 = new Date('20' + frmarr2[2], frmarr2[1], frmarr2[0]);
                            var Toarr2 = ToDate2str.split('/');
                            var ToDate2 = new Date('20' + Toarr2[2], Toarr2[1], Toarr2[0]);

                            $(grd.rows[j].cells[2].children[0]).css('background-color', '');
                            $(grd.rows[j].cells[3].children[0]).css('background-color', '');

                            if ((i < j) && (DeptCity == DeptCity2 && DeptCity2 != "0") && (Arrcity == Arrcity2 && Arrcity2 != "0")) {
                                if ((FrmDate2str == FrmDatestr) || (FrmDate2 > FrmDate && FrmDate2 < ToDate) || (FrmDate2str == ToDatestr)) {
                                    $(value).focus();
                                    alert("Already claimed for this period ");
                                    $(grd.rows[j].cells[2].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else {
                                    $(grd.rows[j].cells[2].children[0]).css('background-color', '');
                                }

                                if ((ToDate2str == FrmDatestr) || (ToDate2 > FrmDate && ToDate2 < ToDate) || (ToDate2str == ToDatestr)) {
                                    $(value).focus();
                                    alert("Already claimed for this period ");
                                    $(grd.rows[j].cells[3].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else { $(grd.rows[j].cells[3].children[0]).css('background-color', ''); }

                            }




                        }




                    }



                    return false;
                }
                else {
                    alert("Please enter employee code.")
                }

            }

            function CheckDuplicatedateFortravelDB(txtBillNo, ArrCity, DeptCity) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
                var BillNo = txtBillNo[0].value;
               
                if (EmpCode != "") {
                    var t = '{ EmpCode:"' + EmpCode + '",BillNo:"' + BillNo + '",ArrCity:"' + ArrCity + '",DeptCity:"' + DeptCity + '" }';
                    $.ajax({

                        type: "POST",
                        url: "/Documents.aspx/checkdateforDbTravelExpense",
                        contentType: "application/json; charset=utf-8",
                        data: t,
                        dataType: "json",
                        success: function (response) {
                            var result = response.d;
                            if (result > 0) {
                                alert("Already claimed for this Bill No.");
                                $(txtBillNo).css('background-color', 'red');
                            }
                            else {
                                $(txtBillNo).css('background-color', '');
                            }
                        },
                        error: function (data) {
                            //Code For hiding preloader
                        }

                    });

                    return false;
                }
                else {
                    alert("Please enter employee code.");
                }

            }
            //== canera - Travel expl. dates duplicity check 
            function CheckDuplicatedateFortravelDBBackuEnd(FrmDatestr, ToDatestr, FrmDate, ToDate, Arrcity, DeptCity) {
                var EmpCode = $('#ContentPlaceHolder1_HDN28142').val();
                //var BillNo = txtBillNo[0].value;

                if (EmpCode != "") {
                    var t = '{ EmpCode:"' + EmpCode + '",Date1:"' + FrmDate + '",Date2:"' + ToDate + '",ArrCity:"' + Arrcity + '",DeptCity:"' + DeptCity + '" }';
                    $.ajax({

                        type: "POST",
                        url: "/Documents.aspx/checkdateforDbTravelExpenseFromDB",
                        contentType: "application/json; charset=utf-8",
                        data: t,
                        dataType: "json",
                        success: function (response) {
                            var result = response.d;
                            if (result > 0) {
                                alert("Already claimed for this from date and to date");
                                $(FrmDatestr).css('background-color', 'red');
                                $(ToDatestr).css('background-color', 'red');
                            }
                            else {
                                $(FrmDatestr).css('background-color', '');
                                $(ToDatestr).css('background-color', '');
                            }
                        },
                        error: function (data) {
                            //Code For hiding preloader
                        }

                    });

                    return false;
                }
                else {
                    alert("Please enter employee code.");
                }

            }
        }
    };
    constructor.Alert();
}
