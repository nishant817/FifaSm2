
//AngularJS
var ngApp = angular.module('fsmApp', ['AxelSoft']);


ngApp.controller('fsmController', ['$scope', '$http', '$filter', '$q', 'playerService', 'friendlyMatchesService', function ($scope, $http, $filter, $q, playerService, friendlyMatchesService) {
  $scope.filteredScores = [];
  $scope.players = [];
  $scope.playersWithAll = [];
  $scope.fScores = [];
  var allScores = [];
  $scope.today = $filter('date')(new Date(), 'yyyy-MM-dd');
  $scope.fMatchDetail = {};
  $scope.teamList = teamList;

  //$scope.state = 'AL';
  //$scope.states = states;
  

  var init = function () {
    var playersPromise = playerService.getPlayers();
    playersPromise.then(function (result) {
      $scope.players = result;
      $scope.playersWithAll = angular.copy(result);
      console.log('Got Players: ' + $scope.playersWithAll.length);
      $scope.playersWithAll.push({ Id: result[result.length - 1].Id + 1, Name: 'All' });
      $scope.filterByPlayer = $scope.playersWithAll[$scope.playersWithAll.length - 1];        
    });

    var friendlyMatchesPromise = friendlyMatchesService.getScores();
    friendlyMatchesPromise.then(function (result) {
      allScores = result;
      console.log('Got scores: ' + result.length);
    });

    $q.all([playersPromise, friendlyMatchesPromise]).then(function () {
      formatFriendlyScores();
    });
  }; //End init()

  init();

  var formatFriendlyScores = function () {
    for (var i = 0; i < allScores.length; i++) {
      allScores[i].mdt = new Date(parseInt(allScores[i].MatchDt.substr(6)));
      allScores[i].n1 = $.grep($scope.players, function (e) { return e.Id == allScores[i].Player1; })[0].Name; //$scope.players.filter(function (v) { return v.Id === fScores[i].p1 })[0].Name;
      allScores[i].n2 = $.grep($scope.players, function (e) { return e.Id == allScores[i].Player2; })[0].Name; //$scope.players.filter(function (v) { return v.Id === fScores[i].p2 })[0].Name;

      if (allScores[i].Goals1 > allScores[i].Goals2) {
        allScores[i].s1 = 'w';
        allScores[i].s2 = 'l';
      }
      else if (allScores[i].Goals1 < allScores[i].Goals2) {
        allScores[i].s1 = 'l';
        allScores[i].s2 = 'w';
      }
      else {
        allScores[i].s1 = 'd';
        allScores[i].s2 = 'd';
      }
    }
    console.log('allScores: ' + allScores);
    $scope.fScores = allScores;
  };  
  
  $scope.setPlayerFilter = function () {
    console.log('Filter by Player: ' + $scope.filterByPlayer);
    if ($scope.filterByPlayer.Name == 'All') {
      $scope.fScores = allScores;
    }
    else {
      $scope.fScores = [];
      console.log('ID: ' + $scope.filterByPlayer.Id);
      console.log('Name: ' + $scope.filterByPlayer.Name);

      var id = $scope.filterByPlayer.Id; //$.grep($scope.players, function (e) { console.log("e= " + e); return e.Name == $scope.filterByPlayer; })[0].Id;
      
      for (var i = 0; i < allScores.length; i++) {
        if (allScores[i].Player1 == id || allScores[i].Player2 == id) {
          $scope.fScores.push(allScores[i]);
        }
      }    
    }
  };
  
  $scope.AddNewScores = function () {
    console.log($scope.fMatchDetail);
    $('#addScoreModal').modal('hide');
    var strData = JSON.stringify($scope.fMatchDetail);
    console.log('stringify fMatchDetail: ' + strData);

    friendlyMatchesService.addScores($scope.fMatchDetail)
      .then(function (result) {
        allScores = result;
        console.log('Added FriendlyScores ' + result.length);
        formatFriendlyScores();
      });
    
  };  

}]);


ngApp.controller('fReportController', ['$scope','$http', '$filter', function ($scope, $http, $filter) {

  var init = function () {
    console.log('inside rptCtrl init');
    $http.get('/Home/GetFriendlyMatchesReports')
    .success(function (data) {
      console.log(data);
      $scope.reports = data;
    })
    .error(function (errMsg) {
      console.log(errMsg);
    });
  };

  init();


  //reports = [players.length];
    
  ////var getReport = function() {
  //  for(var i=0; i< players.length; i++) {
  //    reports[i] = {};
  //    reports[i].pid = players[i].id;
  //    reports[i].name = players[i].name;
  //    reports[i].played = 0
  //    reports[i].wins = 0;
  //    reports[i].lost = 0;
  //    reports[i].draws = 0;
  //    reports[i].gScored = 0;
  //    reports[i].gFaced = 0;
  //  }
    
  //  for(var i=0; i< fScores.length; i++) {
  //    var fsc = fScores[i];
      
  //    reports[fsc.p1].played++;
  //    reports[fsc.p2].played++;
  //    reports[fsc.p1].gScored += parseInt(fsc.g1);
  //    reports[fsc.p1].gFaced += parseInt(fsc.g2);
  //    reports[fsc.p2].gScored += parseInt(fsc.g2);
  //    reports[fsc.p2].gFaced += parseInt(fsc.g1);

  //    if(fsc.g1 > fsc.g2) {
  //      reports[fsc.p1].wins++;
  //      reports[fsc.p2].lost++;
  //    }
  //    else if(fsc.g1 < fsc.g2){
  //      reports[fsc.p1].lost++;
  //      reports[fsc.p2].wins++;
  //    }
  //    else {
  //      reports[fsc.p1].draws++;
  //      reports[fsc.p2].draws++;
  //    }
  //  }
  
  //  $scope.reports = reports;
  
  //}
}]);


ngApp.controller('TournamentController', ['$scope', '$http', '$filter', 'tournamentService', 'playerService', function ($scope, $http, $filter, tournamentService, playerService) {
  $scope.players = [];
  $scope.tournaments = [];
  $scope.tournamentMatches = [];
  var tempTMatches = [];
  $scope.tournamentToAdd = {};
  $scope.selectedTournament = {};
  $scope.matchToAdd = {};
  $scope.matchTypes = ['League', 'Knock Out', 'Qtr Final', 'Semi Final', 'Final'];

  var init = function () {
    var getTournamentsPromose = tournamentService.getTournaments();
    getTournamentsPromose.then(function (result) {
      $scope.tournaments = result;
    });

    var playersPromise = playerService.getPlayers();
    playersPromise.then(function (result) {
      $scope.players = result;
    });
  }; //End Init()
  init();

  $scope.TournamentSelectionChanged = function () {
    console.log($scope.selectedTournament.Id);
    var getTournamentPromise = tournamentService.GetTournamentMatches($scope.selectedTournament.Id);
    getTournamentPromise.then(function (result) {
      tempTMatches = result;
      formatTournamentMatches();
    });

    if ($scope.selectedTournament.HasResult == true) {
      if ($scope.selectedTournament.R1Id > 0) { $scope.selectedTournament.R1Name = $.grep($scope.players, function (e) { return e.Id == $scope.selectedTournament.R1Id; })[0].Name; }
      if ($scope.selectedTournament.R2Id > 0) { $scope.selectedTournament.R2Name = $.grep($scope.players, function (e) { return e.Id == $scope.selectedTournament.R2Id; })[0].Name; }
      if ($scope.selectedTournament.R3Id > 0) { $scope.selectedTournament.R3Name = $.grep($scope.players, function (e) { return e.Id == $scope.selectedTournament.R3Id; })[0].Name; }
      if ($scope.selectedTournament.R4Id > 0) { $scope.selectedTournament.R4Name = $.grep($scope.players, function (e) { return e.Id == $scope.selectedTournament.R4Id; })[0].Name; }
      if ($scope.selectedTournament.R5Id > 0) { $scope.selectedTournament.R5Name = $.grep($scope.players, function (e) { return e.Id == $scope.selectedTournament.R5Id; })[0].Name; }
      if ($scope.selectedTournament.R6Id > 0) { $scope.selectedTournament.R6Name = $.grep($scope.players, function (e) { return e.Id == $scope.selectedTournament.R6Id; })[0].Name; }
      if ($scope.selectedTournament.R7Id > 0) { $scope.selectedTournament.R7Name = $.grep($scope.players, function (e) { return e.Id == $scope.selectedTournament.R7Id; })[0].Name; }
    };
    document.getElementById('tournamentMatchesTbl').style.display = 'block';
  };

  $scope.AddNewTournament = function () {
    if ($scope.tournamentToAdd.Name == null) {
      return;
    }
    if ($scope.tournamentToAdd.Date == null) {
      $scope.tournamentToAdd.Date = new Date().toDateString();
    }

    $('#addTournamentModal').modal('hide');
    var strData = JSON.stringify($scope.tournamentToAdd);
    console.log(strData);

    var addTournamentPromise = tournamentService.addTournament($scope.tournamentToAdd);
    addTournamentPromise.then(function (result) {
      $scope.tournamentToAdd = {};
      $scope.tournaments = result;
    }, function (error) {
      console.log(error);
      alert('could not add new tournament :-(');
    });
  };

  $scope.AddTournamentMatch = function () {
    $scope.matchToAdd.TId = $scope.selectedTournament.Id;
    console.log($scope.matchToAdd);
    $('#addMatchModal').modal('hide');
    var strData = JSON.stringify($scope.matchToAdd);
    console.log('stringify matchToAdd: ' + strData);

    tournamentService.AddTournamentMatch($scope.matchToAdd).then(function (result) {
      $scope.matchToAdd = {};
      tempTMatches = result;
      console.log('Added Tournament Match ' + result.length);
      formatTournamentMatches();
    });
  };

  $scope.AddTournamentResult = function () {
    var strData = JSON.stringify($scope.selectedTournament);
    console.log(strData);

    $('#addResultModal').modal('hide');

    var addResultPromise = tournamentService.AddTournamentResult($scope.selectedTournament);
    addResultPromise.then(function (result) {
      $scope.tournaments = result;
    }, function (error) {
      console.log(error);
      alert('could not add new tournament :-(');
    });
  };

  $scope.showAddMatchModal = function () {
    console.log($scope.selectedTournament.Id);
    if ($scope.selectedTournament.Id == null) {
      alert('Select a Tournament first');
      return;
    }
    $('#addMatchModal').modal('show');
  };

  $scope.ShowMessage = function (message) {
    alert(message);
  };

  var formatTournamentMatches = function () {
    for (var i = 0; i < tempTMatches.length; i++) {
      tempTMatches[i].DId = i + 1;
      tempTMatches[i].n1 = $.grep($scope.players, function (e) { return e.Id == tempTMatches[i].P1; })[0].Name;
      tempTMatches[i].n2 = $.grep($scope.players, function (e) { return e.Id == tempTMatches[i].P2; })[0].Name;

      if (tempTMatches[i].G1 > tempTMatches[i].G2) {
        tempTMatches[i].s1 = 'w';
        tempTMatches[i].s2 = 'l';
      }
      else if (tempTMatches[i].G1 < tempTMatches[i].G2) {
        tempTMatches[i].s1 = 'l';
        tempTMatches[i].s2 = 'w';
      }
      else {
        tempTMatches[i].s1 = 'd';
        tempTMatches[i].s2 = 'd';
      }
    }
    console.log('tournamentMatches after formatting: ' + tempTMatches);
    $scope.tournamentMatches = tempTMatches;
  };


}]);


 