import 'dart:io';

import 'package:frontend/model/candidate.dart';
import 'package:frontend/model/game.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'dart:async';

class PlayerApi {
  ApiBaseHelper _helper = ApiBaseHelper();

  Future<Game> fetchScrambledTeams(List<Candidate> candidates) async {
    final Map<String, dynamic> response =
        await _helper.post("Player", candidates);
    return Game.fromJson(response);
  }
}

class ApiBaseHelper {
  final String _baseUrl = "https://fairteamsai.backend.entertainment720.eu/";

  Future<dynamic> post(String url, List<Candidate> candidates) async {
    final content = jsonEncode(candidates);
    print("Api Post, url $url");
    var responseJson;
    try {
      final response = await http.post(
        _baseUrl + url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8'
        },
        body: content,
      );
      responseJson = _returnResponse(response);
    } on SocketException {
      print("no internet connection");
      throw Exception('No Internet connection');
    }

    print("api post received");
    return responseJson;
  }

  _returnResponse(http.Response response) {
    switch (response.statusCode) {
      case 200:
        var responseJson = json.decode(response.body.toString());
        return responseJson;

      default:
        throw Exception(
            "Error occured while Communication with Server with StatusCode : ${response.statusCode}");
    }
  }
}
