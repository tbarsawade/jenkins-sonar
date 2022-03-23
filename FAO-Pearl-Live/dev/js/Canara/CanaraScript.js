function DocumentCanaraController() {
    var constructor = {
        Alert: function () {

            // POC Canera HSBC  by cloning ===================== Starts 29-12-16

            // ===== for POC Canera HSBC  - Other Expenses child item ===== Start
            var grid = document.getElementById("ContentPlaceHolder1_GRD31252");
            $("#ContentPlaceHolder1_GRD31252 td:first-child input[type=text]").bind("change", function (e) {
                CalculateExpense_POC($(this), "expenseDate");
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31252 td:nth-child(2) select").bind("change", function (e) {
                CalculateExpense_POC($(this), "expenseType");
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31252 td:nth-child(3) input[type=text]").bind("change", function (e) {
                CalculateExpense_POC($(this), "claimAmt");
                e.preventDefault();
                return false;
            });
            //For Personal Expense Claim
            $("#ContentPlaceHolder1_GRD31252 td:nth-child(8) input[type=text]").bind("change", function (e) {
                CalculateExpense_POC($(this), "PersonalExpense");
                e.preventDefault();
                return false;
            });

            // ===== for POC Canera HSBC  - Other Expenses child item ===== End

            //// ===== for POC Canera HSBC  - Lodging Expense Claim child item ===== start
            var grid = document.getElementById("ContentPlaceHolder1_GRD31250");
            $("#ContentPlaceHolder1_GRD31250 td:first-child select").bind("change", function (e) {
                //alert("Place of Stay");
                CalculateLodgingExpense_POC();
            });
            $("#ContentPlaceHolder1_GRD31250 td:nth-child(2) select").bind("change", function (e) {
                //alert("Stay Type");
                CalculateLodgingExpense_POC();
            });
            $("#ContentPlaceHolder1_GRD31250 td:nth-child(4) input[type=text]").bind("change", function (e) {
                //alert("from date");
                CalculateLodgingExpense_POC();
            });
            $("#ContentPlaceHolder1_GRD31250 td:nth-child(5) input[type=text]").bind("change", function (e) {
                //alert("to date");
                CalculateLodgingExpense_POC();
            });
            $("#ContentPlaceHolder1_GRD31250 td:nth-child(6) input[type=text]").bind("change", function (e) {
                //alert("claim amount");
                CalculateLodgingExpense_POC();
            });
            //// ===== for POC Canera HSBC  - Lodging Expense Claim child item ===== End

            //POC Canera HSBC  Duplicate claims =========================================
            $("#ContentPlaceHolder1_GRD31250 td:first-child input[type=text]").bind("change", function (e) {
                CheckDuplicatefordate_POC();
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31250 td:nth-child(3) select").bind("change", function (e) {
                //   CheckDuplicatefordateDB($(this));
                CheckDuplicatefordate_POC($(this));

                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31250 td:nth-child(4) input[type=text]").bind("change", function (e) {
                // CheckDuplicatefordateDB($(this));
                CheckDuplicatefordate_POC($(this));

                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31250 td:nth-child(5) input[type=text]").bind("change", function (e) {
                //  CheckDuplicatefordateDB($(this));
                CheckDuplicatefordate_POC($(this));

                e.preventDefault();
                return false;
            });
            //POC Canera HSBC  Duplicate claims =========================================

            // POC Canera HSBC  duplicate bill no. - mobile datacard
            $("#ContentPlaceHolder1_GRD31252 td:nth-child(5) input[type=text]").bind("change", function (e) {
                checkDuplicateBillNo_POC($(this));
                e.preventDefault();
                return false;
            });

            //POC Canera HSBC Duplicate claims =========================================
            $("#ContentPlaceHolder1_GRD31249 td:first-child select").bind("change", function (e) {
                CheckDuplicatedateFortravel_POC($(this));
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31249 td:nth-child(2) select").bind("change", function (e) {
                CheckDuplicatedateFortravel_POC($(this));
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31249 td:nth-child(3) input[type=text]").bind("change", function (e) {
                CheckDuplicatedateFortravel_POC($(this));
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31249 td:nth-child(4) input[type=text]").bind("change", function (e) {
                CheckDuplicatedateFortravel_POC($(this));
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31249 td:nth-child(7) input[type=text]").bind("change", function (e) {
                checkBillNofor_trvl_Conv_Lodging_POC($(this), 'Travel Expense Claim');
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31250 td:nth-child(8) input[type=text]").bind("change", function (e) {
                alert('Lodging Expense Claim');
                checkBillNofor_trvl_Conv_Lodging_POC($(this), 'Lodging Expense Claim');
                e.preventDefault();
                return false;
            });
            $("#ContentPlaceHolder1_GRD31251 td:nth-child(10) input[type=text]").bind("change", function (e) {
                checkBillNofor_trvl_Conv_Lodging_POC($(this), 'Conveyance Expense Claim');
                e.preventDefault();
                return false;
            });
            // POC Canera HSBC  HSBC POC by cloning =====================  Ends   29-12-16



            // ======= === POC Canerahsbc by cloning - Start 29-12-16


            // for POC Canerahsbc 
            function CalculateExpense_POC(value, text) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();
                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD31252");
                    var grdlenght = grd.rows.length;
                    var person = [], Business = [], valMobileOrDatacard = [];
                    for (var i = 1; i < (grdlenght - 1) ; i++) {
                        var objexpenseDate = grd.rows[i].cells[0].children[0];
                        var objexpenseType = grd.rows[i].cells[1].children[0];
                        var objclaimAmt = grd.rows[i].cells[2].children[0];
                        var objpersonalExpense = grd.rows[i].cells[7].children[0];
                        var objFinalPaidAmount = grd.rows[i].cells[8].children[0];
                        var objJustification = grd.rows[i].cells[9].children[0];
                        var expenseDate = objexpenseDate.value;
                        var expenseType = objexpenseType.value;
                        var claimAmt = objclaimAmt.value;
                        var Justification = objJustification.value;
                        var finalPaidAmount = objFinalPaidAmount.value;
                        var objpersonalExpenseAmount = objpersonalExpense.value;
                        $(objclaimAmt).css('background-color', '');
                        $(objJustification).css('background-color', '');
                        //For Mobile expense should be 10% of claim amount in case of Mobile Expense
                        //if (expenseType == "1254312") {
                        //    if ((claimAmt>0)&&(objpersonalExpenseAmount == "0" || objpersonalExpenseAmount == "")) {
                        //        $(objpersonalExpense).css('background-color', 'red');
                        //        alert("Personal Expense (Mobile Claim) should be 10 % of claim amount at row " + i);
                        //    } else {
                        //        if (Number(objpersonalExpenseAmount) < Number(((claimAmt * 10) / 100))) {
                        //            $(objpersonalExpense).css('background-color', 'red');
                        //            alert("Personal Expense (Mobile Claim) should be 10 % of claim amount  at row " + i);
                        //        } else {
                        //            $(objpersonalExpense).css('background-color', '');
                        //        }
                        //    }
                        //}
                        //For Mobile expense should be 10% of claim amount in case of Mobile Expense
                        if (expenseType == "1254308" || expenseType == "1254312") {
                            var arrexpenseDate = expenseDate.split("/");
                            var monthYear = arrexpenseDate[1] + "/" + arrexpenseDate[2];
                            var arr = [];
                            arr.push(monthYear);
                            arr.push(claimAmt);
                            arr.push(objclaimAmt);
                            arr.push(objJustification);
                            arr.push(expenseType);
                            arr.push(objpersonalExpense);
                            person.push(arr);

                            // new
                            var arr1 = [];

                            //arr1.push(monthYear)
                            //arr1.push(expenseType);
                            //valMobileOrDatacard.push(arr1);

                            //check duplication Mobile and Data card================
                            var expTypeText = "";
                            if (expenseType == "1254308") {
                                arr1 = [];
                                expTypeText = "Data Card Expenses";
                                arr1.push(monthYear)
                                arr1.push(expTypeText);
                                arr1.push(claimAmt);
                                valMobileOrDatacard.push(arr1);
                            }
                            else {
                                arr1 = [];
                                expTypeText = "Mobile Expenses";
                                arr1.push(monthYear)
                                arr1.push(expTypeText);
                                arr1.push(claimAmt);
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
                        if (expenseType == "1254306") {
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
                            async: false,
                            success: function (response) {
                                var result = response.d;
                                var ds = JSON.parse(result);
                                for (var x = 0; x < MobileoutputArr.length; x++) {
                                    var flage = 0;
                                    var IsDeviationflage = 0;
                                    if (MobileoutputArr[x][1] > 1000) {
                                        IsDeviationflage = 1;
                                        $("#ContentPlaceHolder1_fld31958").prop("selectedIndex", 2);
                                        $("#ContentPlaceHolder1_fld31958").css('background-color', 'yellow')
                                        arrtxtobject = $.grep(person, function (d) { return d[0] == MobileoutputArr[x][0]; })
                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                            if (arrtxtobject[z][3].value == "") {
                                                $(arrtxtobject[z][3]).css('background-color', 'red');
                                                //alert("Please fill Justification becuase your mobile limit exceed to 1000 for the month");
                                                $(arrtxtobject[z][3]).focus();
                                            }
                                            else {
                                                $(arrtxtobject[z][3]).css('background-color', '');
                                                alert("clear");
                                            }
                                            //alert('Mobile Expense Claimed can not be more than 1000');
                                        }
                                        alert("Mobile claim amount must be less than or equal to 1000 and fill Justification.");

                                        flage = 1;
                                        //$(objclaimAmt).css('background-color', 'red');
                                        //break;
                                    }
                                    else {
                                        if (IsDeviationflage == 0) {
                                            $("#ContentPlaceHolder1_fld31958").prop("selectedIndex", 1);
                                            $("#ContentPlaceHolder1_fld31958").css('background-color', '')

                                        }
                                    }
                                    if (flage == 0) {
                                        for (var y = 0; y < ds.length; y++) {
                                            if (MobileoutputArr[x][0] == ds[y].ExpDate) {
                                                if ((MobileoutputArr[x][1] + ds[y].ClaimAmt) > 1000) {
                                                    $("#ContentPlaceHolder1_fld31958").prop("selectedIndex", 2);
                                                    $("#ContentPlaceHolder1_fld31958").css('background-color', 'yellow')
                                                    //alert("Mobile claim amount must be less than or equal to 1000.");
                                                    arrtxtobject = $.grep(person, function (d) { return d[0] == MobileoutputArr[x][0]; })
                                                    for (var z = 0; z < arrtxtobject.length; z++) {
                                                        $(arrtxtobject[z][2]).css('background-color', 'red');
                                                        if (arrtxtobject[z][3].value == "") {
                                                            $(arrtxtobject[z][3]).css('background-color', 'red');
                                                            //alert("Please fill Justification becuase your mobile limit exceed to 1000 for the month");
                                                            $(arrtxtobject[z][3]).focus();
                                                        }
                                                        else {
                                                            $(arrtxtobject[z][3]).css('background-color', '');
                                                        }
                                                    }
                                                    alert("Mobile claim amount must be less than or equal to 1000 and fill Justification..");
                                                    //break;
                                                }
                                                else {
                                                    if (IsDeviationflage == 0) {
                                                        $("#ContentPlaceHolder1_fld31958").prop("selectedIndex", 1);
                                                        $("#ContentPlaceHolder1_fld31958").css('background-color', '')
                                                        $(arrtxtobject[z][3]).css('background-color', '');
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
                    //for 10% condition
                    if (person.length > 0) {
                        for (var k = 0; k < person.length; k++) {
                            if (person[k][4] == "1254312") {
                                if ((person[k][1] > 0) && (person[k][5].value == "0" || person[k][5].value == "")) {
                                    $(person[k][5]).css('background-color', 'red');
                                    alert("Personal Expense (Mobile Claim) should be 10 % of claim amount at row " + parseInt(k) + 1);
                                }
                                else {
                                    if (Number(person[k][5].value) < Number((person[k][1] * 10) / 100)) {
                                        $(person[k][5]).css('background-color', 'red');
                                        alert("Personal Expense (Mobile Claim) should be 10 % of claim amount at row " + parseInt(k) + 1);
                                    }
                                    else {
                                        $(person[k][5]).css('background-color', '');
                                    }
                                }
                            }
                        }
                    }
                    //for 10% condition

                   

                    //if (expenseType == "1254312") {
                    //    if ((claimAmt>0)&&(objpersonalExpenseAmount == "0" || objpersonalExpenseAmount == "")) {
                    //        $(objpersonalExpense).css('background-color', 'red');
                    //        alert("Personal Expense (Mobile Claim) should be 10 % of claim amount at row " + i);
                    //    } else {
                    //        if (Number(objpersonalExpenseAmount) < Number(((claimAmt * 10) / 100))) {
                    //            $(objpersonalExpense).css('background-color', 'red');
                    //            alert("Personal Expense (Mobile Claim) should be 10 % of claim amount  at row " + i);
                    //        } else {
                    //            $(objpersonalExpense).css('background-color', '');
                    //        }
                    //    }
                    //}
                    return false;
                    // Now add functionality for Mobile Expense Claim
                  
                }
                else {
                    alert("Please enter employee code.")
                }
            }
            // for POC Canerahsbc


            // for POC Canerahsbc
            function CalculateLodgingExpense_POC() {
                var grd = document.getElementById("ContentPlaceHolder1_GRD31250");
                var grdlenght = grd.rows.length;
                var person = [], Business = [];
                var EmpGrade = document.getElementById("ContentPlaceHolder1_fld31264").value;
                for (var i = 1; i < (grdlenght - 1) ; i++) {
                    var objPlaceOfStay = grd.rows[i].cells[0].children[0];
                    var objStayType = grd.rows[i].cells[1].children[0];
                    var objexpenseFDate = grd.rows[i].cells[3].children[0];
                    var objexpenseToDate = grd.rows[i].cells[4].children[0];
                    var objclaimAmt = grd.rows[i].cells[5].children[0];
                    //=========================================================
                    var PlaceOfStay = objPlaceOfStay.value;
                    var StayType = objStayType.value;
                    var expenseFDate = objexpenseFDate.value;
                    var expenseToDate = objexpenseToDate.value;
                    var claimAmt = objclaimAmt.value;
                    var diffDays = CalculateLodgingExpense_Canra_Day_POC(expenseFDate, expenseToDate, StayType);
                    if (StayType == "1255570") {
                        if (PlaceOfStay == "1254823" || PlaceOfStay == "1254503" || PlaceOfStay == "1254482" ||
                            PlaceOfStay == "1254574" || PlaceOfStay == "1254425" || PlaceOfStay == "1254651" ||
                            PlaceOfStay == "1254585" || PlaceOfStay == "1254692" || PlaceOfStay == "1255063" ||
                            PlaceOfStay == "1254997" || PlaceOfStay == "1254461" || PlaceOfStay == "1255350" ||
                            PlaceOfStay == "1255089" || PlaceOfStay == "1254478" || PlaceOfStay == "1255129" ||
                            PlaceOfStay == "1254462" || PlaceOfStay == "1255420" || PlaceOfStay == "1254501" ||
                            PlaceOfStay == "1255044" || PlaceOfStay == "1254370" || PlaceOfStay == "1254374" ||
                            PlaceOfStay == "1255278" || PlaceOfStay == "1254576" || PlaceOfStay == "1254908" ||
                            PlaceOfStay == "1255279" || PlaceOfStay == "1255050" || PlaceOfStay == "1254649" ||
                            PlaceOfStay == "1255310" || PlaceOfStay == "1254536" || PlaceOfStay == "1254517" ||
                            PlaceOfStay == "1254424" || PlaceOfStay == "1254578") {

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
                    if (StayType == "1255573") {
                        if (PlaceOfStay == "1254823" || PlaceOfStay == "1254503" || PlaceOfStay == "1254482" ||
                            PlaceOfStay == "1254574" || PlaceOfStay == "1254425" || PlaceOfStay == "1254651" ||
                            PlaceOfStay == "1254585" || PlaceOfStay == "1254692" || PlaceOfStay == "1255063" ||
                            PlaceOfStay == "1254997" || PlaceOfStay == "1254461" || PlaceOfStay == "1255350" ||
                            PlaceOfStay == "1255089" || PlaceOfStay == "1254478" || PlaceOfStay == "1255129" ||
                            PlaceOfStay == "1254462" || PlaceOfStay == "1255420" || PlaceOfStay == "1254501" ||
                            PlaceOfStay == "1255044" || PlaceOfStay == "1254370" || PlaceOfStay == "1254374" ||
                            PlaceOfStay == "1255278" || PlaceOfStay == "1254576" || PlaceOfStay == "1254908" ||
                            PlaceOfStay == "1255279" || PlaceOfStay == "1255050" || PlaceOfStay == "1254649" ||
                            PlaceOfStay == "1255310" || PlaceOfStay == "1254536" || PlaceOfStay == "1254517" ||
                            PlaceOfStay == "1254424" || PlaceOfStay == "1254578") {

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
                    if (StayType == "1255571") {
                        if (PlaceOfStay == "1254823" || PlaceOfStay == "1254503" || PlaceOfStay == "1254482" ||
                            PlaceOfStay == "1254574" || PlaceOfStay == "1254425" || PlaceOfStay == "1254651" ||
                            PlaceOfStay == "1254585" || PlaceOfStay == "1254692" || PlaceOfStay == "1255063" ||
                            PlaceOfStay == "1254997" || PlaceOfStay == "1254461" || PlaceOfStay == "1255350" ||
                            PlaceOfStay == "1255089" || PlaceOfStay == "1254478" || PlaceOfStay == "1255129" ||
                            PlaceOfStay == "1254462" || PlaceOfStay == "1255420" || PlaceOfStay == "1254501" ||
                            PlaceOfStay == "1255044" || PlaceOfStay == "1254370" || PlaceOfStay == "1254374" ||
                            PlaceOfStay == "1255278" || PlaceOfStay == "1254576" || PlaceOfStay == "1254908" ||
                            PlaceOfStay == "1255279" || PlaceOfStay == "1255050" || PlaceOfStay == "1254649" ||
                            PlaceOfStay == "1255310" || PlaceOfStay == "1254536" || PlaceOfStay == "1254517" ||
                            PlaceOfStay == "1254424" || PlaceOfStay == "1254578") {

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
                    // for Re-allocation
                    if (StayType == "1257552") {
                        if (EmpGrade == "CL1" || EmpGrade == "CL2" || EmpGrade == "CL3" ||
                            EmpGrade == "CL4" || EmpGrade == "CL5" || EmpGrade == "CL6") {

                            var CalClamAmount = 5000;
                            if (claimAmt > CalClamAmount) {
                                //   alert("Hotel clam amount less than " + CalClamAmount + " for metros citi.");
                                $(objclaimAmt).css('background-color', 'red');
                                //     break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                        }
                        else {
                            var CalClamAmount = 10000;
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

            function CalculateLodgingExpense_Canra_Day_POC(FromDate, ToDate, stayType) {
                arrFromDate = FromDate.split('/');
                arrToDate = ToDate.split('/');
                var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
                var firstDate = new Date('20' + arrFromDate[2], arrFromDate[1], arrFromDate[0]);
                var secondDate = new Date('20' + arrToDate[2], arrToDate[1], arrToDate[0]);
                var diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
                if (stayType != 1255573) {
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
            function CalculateDay_POC(FromDate, ToDate) {
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
            function CheckDuplicatefordate_POC(value) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();

                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD31250");
                    var grdlenght = grd.rows.length;
                    var person = [], Business = [];
                    var isbreak = 0;
                    for (var i = 1; i < (grdlenght - 1) ; i++) {
                        if (isbreak != 0)
                        { break; }
                        var FrmDatestr = grd.rows[i].cells[3].children[0].value;
                        var ToDatestr = grd.rows[i].cells[4].children[0].value;
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

                        if (StayType == "1255570" || StayType == "1255571") {
                            CheckDuplicatefordateDB_POC(value);
                        }
                        else if (StayType == "1255573") {
                            CheckDuplicatefordateDBFoodExpense_POC(value);
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

                            if ((i < j) && (StayType == "1255570" || StayType == "1255571") && (StayType2 == "1255570" || StayType2 == "1255571")) {

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
                            else if ((i < j) && StayType == "1255573" && StayType2 == "1255573") {
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

            function CheckDuplicatefordateDB_POC(txtdate) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();
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

            function CheckDuplicatefordateDBFoodExpense_POC(txtdate) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();
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

            function CheckDuplicatefordateFoodExpense_POC(value) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();

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

                        if (StayType == "1255573") {
                            CheckDuplicatefordateDBFoodExpense_POC(value);
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

                            if ((i < j) && (StayType == "1255573") && (StayType2 == "1255573")) {
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
            function checkDuplicateBillNo_POC(value) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();
                $(value).css('background-color', '');
                CheckDuplicateBillNoDB_POC(value);

                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD31252");
                    var grdlenght = grd.rows.length;

                    var isbreak = 0;
                    for (var i = 1; i < (grdlenght - 1) ; i++) {

                        var BillNo = grd.rows[i].cells[4].children[0].value;
                        var Type = grd.rows[i].cells[1].children[0].value;
                        if (isbreak != 0) {
                            break;
                        }

                        for (var j = 2; j < (grdlenght - 1) ; j++) {

                            var BillNo2 = grd.rows[j].cells[4].children[0].value;
                            var Type2 = grd.rows[i].cells[1].children[0].value;

                            if ((i < j) && BillNo2 != "") {

                                if (($.trim(BillNo) == $.trim(BillNo2)) && (Type == "1254312" || Type == "1254308") && (Type2 == "1254312" || Type2 == "1254308")) {
                                    $(value).focus();
                                    alert("Already claimed for this Bill No. ");
                                    $(grd.rows[j].cells[4].children[0]).css('background-color', 'red');
                                    isbreak = 1;
                                    break;
                                }
                                else { $(grd.rows[j].cells[4].children[0]).css('background-color', ''); }
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
            // done
            function CheckDuplicateBillNoDB_POC(txtBillNo) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();
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
                            if (ds.length > 0) {
                                alert("Already claimed for this Bill No. with Claim ID " + ds[0]["Claim ID"]);
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
            function checkBillNofor_trvl_Conv_Lodging_POC(value, type) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();
                // var BillNo = value[0].value;

                if (EmpCode != "") {

                    if (type == 'Travel Expense Claim') {
                        var grd = document.getElementById("ContentPlaceHolder1_GRD31249");
                        var grdlenght = grd.rows.length;

                        var isbreak = 0;
                        for (var i = 1; i < (grdlenght - 1) ; i++) {

                            var BillNo = grd.rows[i].cells[7].children[0].value;
                            var CompareTypeVal = grd.rows[i].cells[4].children[0].value;
                            var departureCity = grd.rows[i].cells[0].children[0].value;
                            var arrivalCity = grd.rows[i].cells[1].children[0].value;

                            var txtBillNo = grd.rows[i].cells[7].children[0];
                            if (CompareTypeVal == "0" && BillNo != "") {

                                $(grd.rows[i].cells[4].children[0]).css('background-color', 'red');
                                alert("Please select Mode Of Travel");
                                return;
                            }
                            else if (CompareTypeVal == "0" && departureCity != "0" && arrivalCity != "0") {
                                $(grd.rows[i].cells[4].children[0]).css('background-color', 'red');
                                alert("Please select Mode Of Travel");
                                return;
                            }
                            else {
                                // $(grd.rows[i].cells[6].children[0]).html('');
                                $(grd.rows[i].cells[4].children[0]).css('background-color', '');
                            }
                            checkBillNofor_trvl_Conv_LodgingDB_POC(EmpCode, BillNo, txtBillNo, 'Travel Expense Claim', CompareTypeVal);
                            if (isbreak != 0) {
                                break;
                            }
                            for (var j = 2; j < (grdlenght - 1) ; j++) {

                                var BillNo2 = grd.rows[j].cells[7].children[0].value;
                                var CompareTypeVal2 = grd.rows[j].cells[4].children[0].value;
                                if ((i < j) && BillNo2 != "") {

                                    if (($.trim(BillNo) == $.trim(BillNo2)) && CompareTypeVal == CompareTypeVal2) {
                                        $(value).focus();
                                        alert("Already claimed for this Bill No. ");
                                        $(grd.rows[j].cells[7].children[0]).css('background-color', 'red');
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
                        var grd = document.getElementById("ContentPlaceHolder1_GRD31250");
                        var grdlenght = grd.rows.length;

                        var isbreak = 0;
                        for (var i = 1; i < (grdlenght - 1) ; i++) {

                            var BillNo = grd.rows[i].cells[7].children[0].value;
                            var CompareTypeVal = grd.rows[i].cells[1].children[0].value;

                            var txtBillNo = grd.rows[i].cells[7].children[0];
                            if (CompareTypeVal == "0" && BillNo != "") {
                                alert("Please select Stay Type");
                                $(grd.rows[i].cells[1].children[0]).css('background-color', 'red');
                                return;
                            }
                            else {
                                // $(txtBillNo).val('');
                                $(grd.rows[i].cells[1].children[0]).css('background-color', '');
                            }
                            checkBillNofor_trvl_Conv_LodgingDB_POC(EmpCode, BillNo, txtBillNo, 'Lodging Expense Claim', CompareTypeVal);
                            if (isbreak != 0) {
                                break;
                            }
                            for (var j = 2; j < (grdlenght - 1) ; j++) {

                                var BillNo2 = grd.rows[j].cells[7].children[0].value;
                                var CompareTypeVal2 = grd.rows[j].cells[1].children[0].value;
                                if ((i < j) && BillNo2 != "") {

                                    if (($.trim(BillNo) == $.trim(BillNo2)) && CompareTypeVal == CompareTypeVal2) {
                                        $(value).focus();
                                        alert("Already claimed for this Bill No. ");
                                        $(grd.rows[j].cells[7].children[0]).css('background-color', 'red');
                                        isbreak = 1;
                                        break;
                                    }
                                    else { $(grd.rows[j].cells[7].children[0]).css('background-color', ''); }
                                }
                            }
                        }

                        return false;
                    }
                    else if (type == 'Conveyance Expense Claim') {
                        var grd = document.getElementById("ContentPlaceHolder1_GRD31251");
                        var grdlenght = grd.rows.length;

                        var isbreak = 0;
                        for (var i = 1; i < (grdlenght - 1) ; i++) {

                            var BillNo = grd.rows[i].cells[9].children[0].value;
                            var CompareTypeVal = grd.rows[i].cells[2].children[0].value;

                            var txtBillNo = grd.rows[i].cells[9].children[0];
                            if (CompareTypeVal == "0" && BillNo != "") {
                                alert("Please select Conveyance Type");
                                $(grd.rows[i].cells[2].children[0]).css('background-color', 'red');
                                return;
                            }
                            else {
                                // $(txtBillNo).val('');
                                $(grd.rows[i].cells[2].children[0]).css('background-color', '');
                            }
                            checkBillNofor_trvl_Conv_LodgingDB_POC(EmpCode, BillNo, txtBillNo, 'Conveyance Expense Claim', CompareTypeVal);
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


            function checkBillNofor_trvl_Conv_LodgingDB_POC(EmpCode, BillNo, value, type, CompareTypeVal) {
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

            function CheckDuplicatedateFortravel_POC(value) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();

                if (EmpCode != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD31249");
                    var grdlenght = grd.rows.length;
                    var grdIndex = value[0].id.split("_");
                    var finalIndex = Number(grdIndex[grdIndex.length - 1]) + 1;
                    CheckDuplicatedateFortravelDB_POC(value, grd.rows[finalIndex].cells[1].children[0].value, grd.rows[finalIndex].cells[0].children[0].value);
                    CheckDuplicatedateFortravelDBBackuEnd_POC(FrmDatestr, ToDatestr, FrmDate, ToDate, grd.rows[finalIndex].cells[1].children[0].value, grd.rows[finalIndex].cells[0].children[0].value)
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

            function CheckDuplicatedateFortravelDB_POC(txtBillNo, ArrCity, DeptCity) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();
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
            function CheckDuplicatedateFortravelDBBackuEnd_POC(FrmDatestr, ToDatestr, FrmDate, ToDate, Arrcity, DeptCity) {
                var EmpCode = $('#ContentPlaceHolder1_HDN31245').val();
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
            // ======= === POC Canerahsbc by cloning - End 29-12-16 

        }
    };
    constructor.Alert();
}
