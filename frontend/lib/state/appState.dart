import 'package:flutter/widgets.dart';
import 'package:frontend/reducer/gameConfigReducer.dart';
import 'package:frontend/reducer/gameReducer.dart';
import 'package:frontend/state/gameConfigState.dart';
import 'package:frontend/state/gameState.dart';

AppState appReducer(AppState state, dynamic action) => AppState(
      gameConfigState: gameConfigReducer(state.gameConfigState, action),
      gameState: gameReducer(state.gameState, action),
    );

class AppState {
  final GameConfigState gameConfigState;
  final GameState gameState;

  AppState({
    @required this.gameConfigState,
    @required this.gameState,
  });

  factory AppState.initial() {
    return AppState(
        gameConfigState: GameConfigState.initial(),
        gameState: GameState.initial());
  }

  static AppState fromJson(dynamic json) {
    if (json != null) {
      print(json);
      return AppState(
          gameConfigState: GameConfigState.fromJson(json["gameConfigState"]),
          gameState: GameState.fromJson(json["gameState"]));
    } else {
      return AppState(
          gameConfigState: GameConfigState.initial(),
          gameState: GameState.initial());
    }
  }

  dynamic toJson() {
    return {
      "gameConfigState": gameConfigState.toJson(),
      "gameState": gameState.toJson()
    };
  }
}
