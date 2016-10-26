angular.module('fsmApp')

.service('playerService', ['$http', '$q', function ($http, $q) {
  var players = [];

  this.getPlayers = function () {
    if (players.length > 0) {
      //console.log('yay! we already have players' + players.length);
      var deferred = $q.defer();
      var tempPlayers = angular.copy(players);
      deferred.resolve(tempPlayers);
      return deferred.promise;
    }
    else {
      //console.log('Dont have players. fetching it again');
      return $http.get('/Home/GetPlayers')
                  .then(function (result) {
                    players = result.data;
                    return result.data;
                  });
    }
  };
}])

//================================================================================

.service('friendlyMatchesService', ['$http', function ($http) {
  
  this.getScores = function () {
    return $http.get('/Home/GetFriendlyScores')
                .then(function (result) {
                  return result.data;
                });
  };

  this.addScores = function (scoresToAdd) {
    return $http.post('/Home/AddFriendlyScores', scoresToAdd)
                .then(function (result) {
                  return result.data;
                });
  };

  this.getFriendlyMatchesReports = function () {
    return $http.get('/Home/GetFriendlyMatchesReports')
                .then(function (result) {
                  return result.data;
                });
  };
}])

//================================================================================

.service('tournamentService', ['$http', function ($http) {
  this.getTournaments = function () {
    return $http.get('/Tournament/GetTournaments')
          .then(function (result) {
            return result.data;
          }, function (error) {
            console.log(error.data);
          });
  };

  this.GetTournamentMatches = function (tournamentId) {
    return $http.get('/Tournament/GetTournamentMatches', { params: { tournamentId: tournamentId } })
          .then(function (result) {
            return result.data;
          }, function (error) {
            console.log(error.data);
          });
  };

  this.addTournament = function (tournamentToAdd) {
    return $http.post('/Tournament/AddTournament', tournamentToAdd)
          .then(function (result) {
            return result.data;
          }, function (error) {
            console.log(error.data);
          });
  };

  this.AddTournamentMatch = function (matchToAdd) {
    return $http.post('/Tournament/AddTournamentMatch', matchToAdd)
            .then(function (result) {
              return result.data;
            });
  };

  this.AddTournamentResult = function (selectedTournament) {
    return $http.post('/Tournament/AddTournamentResult', selectedTournament)
          .then(function (result) {
            return result.data;
          }, function (error) {
            console.log(error.data);
          });
  };


}])

;