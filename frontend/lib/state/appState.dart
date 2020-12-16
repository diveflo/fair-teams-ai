import 'package:flutter/widgets.dart';
import 'package:frontend/reducer/gameReducer.dart';
import 'package:frontend/state/gameState.dart';

AppState appReducer(AppState state, dynamic action) => AppState(
      gameState: gameReducer(state.gameState, action),
    );

class AppState {
  final GameState gameState;

  AppState({@required this.gameState});

  factory AppState.initial() {
    return AppState(gameState: GameState.initial());
  }
}
