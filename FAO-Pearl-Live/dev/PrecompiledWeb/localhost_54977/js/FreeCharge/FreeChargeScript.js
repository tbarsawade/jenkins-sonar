function DocumentFreeChargeController() {
    var constructor = {
        Alert: function () {
            //// ===== for Freecharge - Lodging expense  child item ===== Start
            var grid = document.getElementById("ContentPlaceHolder1_GRD27519");
            $("#ContentPlaceHolder1_GRD27519 td:first-child select").bind("change", function (e) {
                //alert("Place of Stay");
                CalculateLodgingExpense_Freecharge();
            });
            $("#ContentPlaceHolder1_GRD27519 td:nth-child(2) select").bind("change", function (e) {
                //alert("Stay Type");
                CalculateLodgingExpense_Freecharge();
            });
            $("#ContentPlaceHolder1_GRD27519 td:nth-child(3) input[type=text]").bind("change", function (e) {
                //alert("Check in date");
                CalculateLodgingExpense_Freecharge();
            });
            $("#ContentPlaceHolder1_GRD27519 td:nth-child(4) input[type=text]").bind("change", function (e) {
                //alert("Check out date");
                CalculateLodgingExpense_Freecharge();
            });
            $("#ContentPlaceHolder1_GRD27519 td:nth-child(6) input[type=text]").bind("change", function (e) {
                //alert("claim amount");
                CalculateLodgingExpense_Freecharge();
            });
            // ===== for Freecharge - Lodging expense child item ===== End

            //freecharge Convence expense Clam=========================================
            var grid = document.getElementById("ContentPlaceHolder1_GRD27520");
            $("#ContentPlaceHolder1_GRD27520 td:first-child select").bind("change", function (e) {
                //alert("Place of Stay");
                ConvenceexpenseClamForfreecharge("");
            });
            $("#ContentPlaceHolder1_GRD27520 td:nth-child(2) input[type=text]").bind("change", function (e) {
                //alert("Place of Stay");
                ConvenceexpenseClamForfreecharge("");
            });
            $("#ContentPlaceHolder1_GRD27520 td:nth-child(3) select").bind("change", function (e) {
                //alert("from date");
                ConvenceexpenseClamForfreecharge($(this).val());
            });
            $("#ContentPlaceHolder1_GRD27520 td:nth-child(6) input[type=text]").bind("change", function (e) {
                //alert("to date");
                ConvenceexpenseClamForfreecharge("");
               
            });
            //============================================================================================================


            // for Freecharge 
            function CalculateLodgingExpense_Freecharge() {
                var EmpGrade = $('#ContentPlaceHolder1_fld27975').val();

                var grd = document.getElementById("ContentPlaceHolder1_GRD27519");
                var grdlenght = grd.rows.length;
                var person = [], Business = [];
                for (var i = 1; i < (grdlenght - 1) ; i++) {
                    var objPlaceOfStay = grd.rows[i].cells[0].children[0];
                    var objStayType = grd.rows[i].cells[1].children[0];
                    var objexpenseFDate = grd.rows[i].cells[2].children[0];
                    var objexpenseToDate = grd.rows[i].cells[3].children[0];
                    var objclaimAmt = grd.rows[i].cells[5].children[0];
                    //=========================================================
                    var PlaceOfStay = objPlaceOfStay.value;
                    var StayType = objStayType.value;
                    var expenseFDate = objexpenseFDate.value;
                    var expenseToDate = objexpenseToDate.value;
                    var claimAmt = objclaimAmt.value;
                    var diffDays = CalculateDay(expenseFDate, expenseToDate);

                    if (StayType == "1089525") {
                        if (PlaceOfStay == "1089513" || PlaceOfStay == "1089514" || PlaceOfStay == "1089515" ||
                            PlaceOfStay == "1089516" || PlaceOfStay == "1089518" || PlaceOfStay == "1089519" ||
                            PlaceOfStay == "1089520" || PlaceOfStay == "1089522" || PlaceOfStay == "1089524") {

                            var CalClamAmount = 0;
                            var grdmsg = "";
                            if (EmpGrade == "1089435" || EmpGrade == "1089425" || EmpGrade == "1089434" || EmpGrade == "1089428" || EmpGrade == "1089423") {  //L0 to L3 and metro city
                                CalClamAmount = (2500 * diffDays);
                                grdmsg = "(Upto Grade Level L3) per Day is "
                                //    alert("Lodging claim Limit (Upto Grade Level L3) per Day is " + CalClamAmount + "!");
                            }
                            if (EmpGrade == "1089426" || EmpGrade == "1089430" || EmpGrade == "1089379" || EmpGrade == "1089427") { //L4 to L7 and metro city
                                CalClamAmount = (4000 * diffDays);
                                grdmsg = "(Grade L4-L7) per Day is "
                                //   alert("Lodging claim Limit (Grade L4-L7) per Day is " + CalClamAmount + "!");
                            }
                            if (EmpGrade == "1089424" || EmpGrade == "1089432" || EmpGrade == "1089422" || EmpGrade == "1089429") { //L8 to L11 and metro city
                                CalClamAmount = (6000 * diffDays);
                                grdmsg = "(Grade L8-L11) per Day is "
                                //    alert("Lodging claim Limit (Grade L8-L11) per Day is " + CalClamAmount + "!");
                            }
                            if (EmpGrade == "1089431" || EmpGrade == "1089378" || EmpGrade == "1089433") { //L11 and Above and metro city
                                CalClamAmount = (7000 * diffDays);
                                grdmsg = "(Grade L11 and above) per Day is "
                                //   alert("Lodging claim Limit (Grade L11 and above) per Day is " + CalClamAmount + "!");
                            }

                            if (claimAmt > CalClamAmount) {
                                // alert("Hotel clam amount less than " + CalClamAmount + " for metros citi.");
                                alert("Lodging claim Limit Exceeded per Day is " + CalClamAmount + "!");
                                $(objclaimAmt).css('background-color', 'red');
                                //     break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                        }
                        else {
                            var CalClamAmount = 0;
                            var grdmsg = "";
                            if (EmpGrade == "1089435" || EmpGrade == "1089425" || EmpGrade == "1089434" || EmpGrade == "1089428" || EmpGrade == "1089423") {  //L0 to L3 and metro city
                                CalClamAmount = (2000 * diffDays);
                                grdmsg = "(Upto Grade Level L3) per Day is "
                                // alert("Lodging claim Limit (Upto Grade Level L3) per Day is " + CalClamAmount + "!");
                            }
                            if (EmpGrade == "1089426" || EmpGrade == "1089430" || EmpGrade == "1089379" || EmpGrade == "1089427") { //L4 to L7 and metro city
                                CalClamAmount = (3000 * diffDays);
                                grdmsg = "(Grade L4-L7) per Day is "
                                //    alert("Lodging claim Limit (Grade L4-L7) per Day is " + CalClamAmount + "!");
                            }
                            if (EmpGrade == "1089424" || EmpGrade == "1089432" || EmpGrade == "1089422" || EmpGrade == "1089429") { //L8 to L11 and metro city
                                CalClamAmount = (5000 * diffDays);
                                grdmsg = "(Grade L8-L11) per Day is "
                                //  alert("Lodging claim Limit (Grade L8-L11) per Day is " + CalClamAmount + "!");
                            }
                            if (EmpGrade == "1089431" || EmpGrade == "1089378" || EmpGrade == "1089433") { //L11 and Above and metro city
                                CalClamAmount = (6000 * diffDays);
                                grdmsg = "(Grade L11 and above) per Day is "
                                // alert("Lodging claim Limit (Grade L11 and above) per Day is " + CalClamAmount + "!");
                            }

                            if (claimAmt > CalClamAmount) {
                                alert("Lodging claim Limit " + grdmsg + "" + CalClamAmount + "!");
                                $(objclaimAmt).css('background-color', 'red');
                                //     break;
                            }
                            else
                                $(objclaimAmt).css('background-color', '');
                            //var CalClamAmount = (1200 * diffDays);
                            //if (claimAmt > CalClamAmount) {
                            //    //      alert("Hotel clam amount less than " + CalClamAmount + " for other citi.");
                            //    $(objclaimAmt).css('background-color', 'red');
                            //    //     break;
                            //}
                            //else
                            //    $(objclaimAmt).css('background-color', '');
                        }
                    }
                    //=========================================================
                }
            }

            // for Freecharge
            //freecharge Convence expense Clam=========================================
            function ConvenceexpenseClamForfreecharge(Convtype) {
                var EmpGrade = $('#ContentPlaceHolder1_fld27975').val();

                if (EmpGrade != "") {
                    var grd = document.getElementById("ContentPlaceHolder1_GRD27520");
                    var grdlenght = grd.rows.length;
                    var Fourwheeler = [], Twowheeler = [], Taxi = [], AutoRickshaw = [], TwowheelerNonMatro = [];
                    //EmpGrade = "1089425";
                    if (EmpGrade == "1089425" || EmpGrade == "1089435") {
                        if (Convtype == "" || Convtype == "1092359") {
                            for (var i = 1; i < (grdlenght - 1) ; i++) {
                                var objTravelDate = grd.rows[i].cells[1].children[0];
                                var objConveyanceType = grd.rows[i].cells[2].children[0];
                                var objTotalAmount = grd.rows[i].cells[5].children[0];
                                var objTravelingInCity = grd.rows[i].cells[0].children[0];
                                $(objTotalAmount).css('background-color', '');

                                var TravelingInCity = objTravelingInCity.value;
                                var TravelDate = objTravelDate.value;
                                var ConveyanceType = objConveyanceType.value;
                                var TotalAmount = objTotalAmount.value;
                                if (ConveyanceType == "1092359") {
                                    var arrTravelDate = TravelDate.split("/");
                                    var monthYear = arrTravelDate[1] + "/" + arrTravelDate[2];

                                    var arr = [];
                                    if (TravelingInCity == "1089513" || TravelingInCity == "1089514" || TravelingInCity == "1089515" ||
                                    TravelingInCity == "1089516" || TravelingInCity == "1089518" || TravelingInCity == "1089519" ||
                                    TravelingInCity == "1089520" || TravelingInCity == "1089522" || TravelingInCity == "1089524") {

                                        arr.push(monthYear);
                                        arr.push(TotalAmount);
                                        arr.push(objTotalAmount);
                                        Twowheeler.push(arr);
                                    }
                                    else {
                                        arr.push(monthYear);
                                        arr.push(TotalAmount);
                                        arr.push(objTotalAmount);
                                        TwowheelerNonMatro.push(arr);
                                    }
                                }
                                //else {
                                //    alert("Only two wheeler applicable for Grade L2 and below ");
                                //    break;
                                //}
                            }
                        }
                        else {
                            alert("Only two wheeler applicable for Grade L2 and below ");
                        }

                        if (Twowheeler.length > 0) {
                            var items = {}, base, key;
                            for (var i = 0; i < Twowheeler.length; i++) {
                                base = Twowheeler[i];
                                key = base[0];
                                if (!items[key]) {
                                    items[key] = 0;
                                }
                                items[key] += parseInt(base[1]);
                            }

                            // Now, generate new array
                            var TwowheeleroutputArr = [], temp;
                            for (key in items) {
                                temp = [key, items[key]]
                                TwowheeleroutputArr.push(temp)
                            }

                            var t = '{ documentType: "Conveyance Expense Claim",EmpGrade:"' + EmpGrade + '",ConveyanceType:"Twowheeler" }';
                            $.ajax({
                                type: "POST",
                                url: "/Documents.aspx/GetfreechargedbSum",
                                contentType: "application/json; charset=utf-8",
                                data: t,
                                dataType: "json",
                                success: function (response) {
                                    var result = response.d;
                                    var ds = JSON.parse(result);
                                    for (var x = 0; x < TwowheeleroutputArr.length; x++) {
                                        var flage = 0;
                                        if (TwowheeleroutputArr[x][1] > 5000) {

                                            arrtxtobject = $.grep(Twowheeler, function (d) { return d[0] == TwowheeleroutputArr[x][0]; })
                                            for (var z = 0; z < arrtxtobject.length; z++) {
                                                $(arrtxtobject[z][2]).css('background-color', 'red');
                                            }
                                            //alert("Mobile claim amount must be less than or equal to 1000.");
                                            flage = 1;

                                            //alert("Two Wheeler total amount must be less than or equal to 5000 for matro cities.");
                                            //$(value).focus();
                                            //break;
                                        }
                                        if (flage == 0) {
                                            for (var y = 0; y < ds.length; y++) {
                                                if (TwowheeleroutputArr[x][0] == ds[y].ExpDate) {
                                                    if ((TwowheeleroutputArr[x][1] + ds[y].ClaimAmt) > 5000) {

                                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                                        }

                                                        //alert("Two Wheeler total amount must be less than or equal to 5000 for matro cities.");
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
                        if (TwowheelerNonMatro.length > 0) {
                            var items = {}, base, key;
                            for (var i = 0; i < TwowheelerNonMatro.length; i++) {
                                base = TwowheelerNonMatro[i];
                                key = base[0];
                                if (!items[key]) {
                                    items[key] = 0;
                                }
                                items[key] += parseInt(base[1]);
                            }

                            // Now, generate new array
                            var TwowheelerNonMatrooutputArr = [], temp;
                            for (key in items) {
                                temp = [key, items[key]]
                                TwowheelerNonMatrooutputArr.push(temp)
                            }

                            var t = '{ documentType: "Conveyance Expense Claim",EmpGrade:"' + EmpGrade + '",ConveyanceType:"Twowheeler" }';
                            $.ajax({
                                type: "POST",
                                url: "/Documents.aspx/GetfreechargedbSum",
                                contentType: "application/json; charset=utf-8",
                                data: t,
                                dataType: "json",
                                success: function (response) {
                                    var result = response.d;
                                    var ds = JSON.parse(result);
                                    for (var x = 0; x < TwowheelerNonMatrooutputArr.length; x++) {
                                        var flage = 0;
                                        if (TwowheelerNonMatrooutputArr[x][1] > 3500) {
                                            arrtxtobject = $.grep(TwowheelerNonMatro, function (d) { return d[0] == TwowheelerNonMatrooutputArr[x][0]; })
                                            for (var z = 0; z < arrtxtobject.length; z++) {
                                                $(arrtxtobject[z][2]).css('background-color', 'red');
                                            }
                                            //alert("Mobile claim amount must be less than or equal to 1000.");
                                            flage = 1;

                                            //alert("Two Wheeler total amount must be less than or equal to 3500 for non matro cities.");
                                            //$(value).focus();
                                            //break;
                                        }
                                        if (flage == 0) {
                                            for (var y = 0; y < ds.length; y++) {
                                                if (TwowheelerNonMatrooutputArr[x][0] == ds[y].ExpDate) {
                                                    if ((TwowheelerNonMatrooutputArr[x][1] + ds[y].ClaimAmt) > 3500) {
                                                        arrtxtobject = $.grep(TwowheelerNonMatro, function (d) { return d[0] == TwowheelerNonMatrooutputArr[x][0]; })
                                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                                        }
                                                        //alert("Two Wheeler total amount must be less than or equal to 3500 for non matro cities..");
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

                    }
                    else {
                        for (var i = 1; i < (grdlenght - 1) ; i++) {
                            var objTravelDate = grd.rows[i].cells[1].children[0];
                            var objConveyanceType = grd.rows[i].cells[2].children[0];
                            var objTotalAmount = grd.rows[i].cells[5].children[0];
                            var objTravelingInCity = grd.rows[i].cells[0].children[0];
                            $(objTotalAmount).css('background-color', '');
                            var TravelingInCity = objTravelingInCity.value;
                            var TravelDate = objTravelDate.value;
                            var ConveyanceType = objConveyanceType.value;
                            var TotalAmount = objTotalAmount.value;

                            if (ConveyanceType == "1092358") {
                                var arrTravelDate = TravelDate.split("/");
                                var monthYear = arrTravelDate[1] + "/" + arrTravelDate[2];
                                var arr = [];
                                arr.push(monthYear);
                                arr.push(TotalAmount);
                                arr.push(objTotalAmount);
                                Fourwheeler.push(arr);

                            }
                            if (ConveyanceType == "1092359") {
                                var arrTravelDate = TravelDate.split("/");
                                var monthYear = arrTravelDate[1] + "/" + arrTravelDate[2];
                                var arr = [];
                                arr.push(monthYear);
                                arr.push(TotalAmount);
                                arr.push(objTotalAmount);
                                Twowheeler.push(arr);
                            }
                            if (ConveyanceType == "1092360") {
                                var arrTravelDate = TravelDate.split("/");
                                var monthYear = arrTravelDate[1] + "/" + arrTravelDate[2];
                                var arr = [];
                                arr.push(monthYear);
                                arr.push(TotalAmount);
                                arr.push(objTotalAmount);
                                Taxi.push(arr);
                            }
                            if (ConveyanceType == "1092361") {
                                var arrTravelDate = TravelDate.split("/");
                                var monthYear = arrTravelDate[1] + "/" + arrTravelDate[2];
                                var arr = [];
                                arr.push(monthYear);
                                arr.push(TotalAmount);
                                arr.push(objTotalAmount);
                                AutoRickshaw.push(arr);
                            }
                        }
                        if (Fourwheeler.length > 0) {
                            var items = {}, base, key;
                            for (var i = 0; i < Fourwheeler.length; i++) {
                                base = Fourwheeler[i];
                                key = base[0];
                                if (!items[key]) {
                                    items[key] = 0;
                                }
                                items[key] += parseInt(base[1]);
                            }

                            // Now, generate new array
                            var FourwheeleroutputArr = [], temp;
                            for (key in items) {
                                temp = [key, items[key]]
                                FourwheeleroutputArr.push(temp)
                            }

                            var t = '{ documentType: "Conveyance Expense Claim",EmpGrade:"' + EmpGrade + '",ConveyanceType:"Fourwheeler" }';
                            $.ajax({
                                type: "POST",
                                url: "/Documents.aspx/GetfreechargedbSum",
                                contentType: "application/json; charset=utf-8",
                                data: t,
                                dataType: "json",
                                success: function (response) {
                                    var result = response.d;
                                    var ds = JSON.parse(result);
                                    for (var x = 0; x < FourwheeleroutputArr.length; x++) {
                                        var flage = 0;
                                        if (FourwheeleroutputArr[x][1] > 6000) {

                                            arrtxtobject = $.grep(Fourwheeler, function (d) { return d[0] == FourwheeleroutputArr[x][0]; })
                                            for (var z = 0; z < arrtxtobject.length; z++) {
                                                $(arrtxtobject[z][2]).css('background-color', 'red');
                                            }
                                            //alert("Mobile claim amount must be less than or equal to 1000.");
                                            flage = 1;
                                            //alert("Four Wheeler total amount must be less than or equal to 6000.");
                                            //$(objclaimAmt).css('background-color', 'red');
                                            //break;
                                        }
                                        if (flage == 0) {
                                            for (var y = 0; y < ds.length; y++) {
                                                if (FourwheeleroutputArr[x][0] == ds[y].ExpDate) {
                                                    if ((FourwheeleroutputArr[x][1] + ds[y].ClaimAmt) > 6000) {
                                                        arrtxtobject = $.grep(Fourwheeler, function (d) { return d[0] == FourwheeleroutputArr[x][0]; })
                                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                                        }
                                                        //alert("Four Wheeler total amount must be less than or equal to 6000.");
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
                        if (Twowheeler.length > 0) {
                            var items = {}, base, key;
                            for (var i = 0; i < Twowheeler.length; i++) {
                                base = Twowheeler[i];
                                key = base[0];
                                if (!items[key]) {
                                    items[key] = 0;
                                }
                                items[key] += parseInt(base[1]);
                            }

                            // Now, generate new array
                            var TwowheeleroutputArr = [], temp;
                            for (key in items) {
                                temp = [key, items[key]]
                                TwowheeleroutputArr.push(temp)
                            }

                            var t = '{ documentType: "Conveyance Expense Claim",EmpGrade:"' + EmpGrade + '",ConveyanceType:"Twowheeler" }';
                            $.ajax({
                                type: "POST",
                                url: "/Documents.aspx/GetfreechargedbSum",
                                contentType: "application/json; charset=utf-8",
                                data: t,
                                dataType: "json",
                                success: function (response) {
                                    var result = response.d;
                                    var ds = JSON.parse(result);
                                    for (var x = 0; x < TwowheeleroutputArr.length; x++) {
                                        var flage = 0;
                                        if (TwowheeleroutputArr[x][1] > 3000) {
                                            arrtxtobject = $.grep(Twowheeler, function (d) { return d[0] == TwowheeleroutputArr[x][0]; })
                                            for (var z = 0; z < arrtxtobject.length; z++) {
                                                $(arrtxtobject[z][2]).css('background-color', 'red');
                                            }
                                            flage = 1;
                                            //alert("Two Wheeler total amount must be less than or equal to 3000.");
                                            //$(value).focus();
                                            //break;
                                        }
                                        if (flage == 0) {
                                            for (var y = 0; y < ds.length; y++) {
                                                if (outputArr[x][0] == ds[y].ExpDate) {
                                                    if ((outputArr[x][1] + ds[y].ClaimAmt) > 3000) {
                                                        arrtxtobject = $.grep(Twowheeler, function (d) { return d[0] == TwowheeleroutputArr[x][0]; })
                                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                                        }
                                                        //alert("Two Wheeler total amount must be less than or equal to 3000.");
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
                        if (Taxi.length > 0) {
                            var items = {}, base, key;
                            for (var i = 0; i < Taxi.length; i++) {
                                base = Taxi[i];
                                key = base[0];
                                if (!items[key]) {
                                    items[key] = 0;
                                }
                                items[key] += parseInt(base[1]);
                            }

                            // Now, generate new array
                            var TaxioutputArr = [], temp;
                            for (key in items) {
                                temp = [key, items[key]]
                                TaxioutputArr.push(temp)
                            }

                            var t = '{ documentType: "Conveyance Expense Claim",EmpGrade:"' + EmpGrade + '",ConveyanceType:"Taxi" }';
                            $.ajax({
                                type: "POST",
                                url: "/Documents.aspx/GetfreechargedbSum",
                                contentType: "application/json; charset=utf-8",
                                data: t,
                                dataType: "json",
                                success: function (response) {
                                    var result = response.d;
                                    var ds = JSON.parse(result);
                                    for (var x = 0; x < TaxioutputArr.length; x++) {
                                        var flage = 0;
                                        if (TaxioutputArr[x][1] > 5000) {
                                            arrtxtobject = $.grep(Taxi, function (d) { return d[0] == TaxioutputArr[x][0]; })
                                            for (var z = 0; z < arrtxtobject.length; z++) {
                                                $(arrtxtobject[z][2]).css('background-color', 'red');
                                            }
                                            flage = 1;
                                            //alert("Taxi total amount must be less than or equal to 5000.");
                                            //$(value).focus();
                                            //break;
                                        }
                                        if (flage = 0) {
                                            for (var y = 0; y < ds.length; y++) {
                                                if (TaxioutputArr[x][0] == ds[y].ExpDate) {
                                                    if ((TaxioutputArr[x][1] + ds[y].ClaimAmt) > 5000) {
                                                        arrtxtobject = $.grep(Taxi, function (d) { return d[0] == TaxioutputArr[x][0]; })
                                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                                        }
                                                        //alert("Taxi total amount must be less than or equal to 5000.");
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
                        if (AutoRickshaw.length > 0) {
                            var items = {}, base, key;
                            for (var i = 0; i < AutoRickshaw.length; i++) {
                                base = AutoRickshaw[i];
                                key = base[0];
                                if (!items[key]) {
                                    items[key] = 0;
                                }
                                items[key] += parseInt(base[1]);
                            }

                            // Now, generate new array
                            var AutoRickshawoutputArr = [], temp;
                            for (key in items) {
                                temp = [key, items[key]]
                                AutoRickshawoutputArr.push(temp)
                            }

                            var t = '{ documentType: "Conveyance Expense Claim",EmpGrade:"' + EmpGrade + '",ConveyanceType:"AutoRickshaw" }';
                            $.ajax({
                                type: "POST",
                                url: "/Documents.aspx/GetfreechargedbSum",
                                contentType: "application/json; charset=utf-8",
                                data: t,
                                dataType: "json",
                                success: function (response) {
                                    var result = response.d;
                                    var ds = JSON.parse(result);
                                    for (var x = 0; x < AutoRickshawoutputArr.length; x++) {
                                        var flage = 0;
                                        if (AutoRickshawoutputArr[x][1] > 3500) {
                                            arrtxtobject = $.grep(AutoRickshaw, function (d) { return d[0] == AutoRickshawoutputArr[x][0]; })
                                            for (var z = 0; z < arrtxtobject.length; z++) {
                                                $(arrtxtobject[z][2]).css('background-color', 'red');
                                            }
                                            flage = 1;
                                            //alert("Auto Rickshaw total amount must be less than or equal to 3500.");
                                            //$(value).focus();
                                            //break;
                                        }
                                        if (flage == 0) {
                                            for (var y = 0; y < ds.length; y++) {
                                                if (AutoRickshawoutputArr[x][0] == ds[y].ExpDate) {
                                                    if ((AutoRickshawoutputArr[x][1] + ds[y].ClaimAmt) > 3500) {
                                                        arrtxtobject = $.grep(AutoRickshaw, function (d) { return d[0] == AutoRickshawoutputArr[x][0]; })
                                                        for (var z = 0; z < arrtxtobject.length; z++) {
                                                            $(arrtxtobject[z][2]).css('background-color', 'red');
                                                        }
                                                        //alert("Auto Rickshaw total amount must be less than or equal to 3500.");
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
                    }
                    return false;
                }
                else {
                    alert("Please enter employee code.")
                }
            }
            //=================================================================================


        }
    };
    constructor.Alert();
}
$(document).ready(function () {
    var documentFreeChargeController = new DocumentFreeChargeController();
});