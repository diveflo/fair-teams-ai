import 'package:flutter/widgets.dart';
import 'package:frontend/reducer/gameReducer.dart';
import 'package:frontend/state/gameConfigState.dart';

AppState appReducer(AppState state, dynamic action) => AppState(
      gameConfigState: gameReducer(state.gameConfigState, action),
    );

class AppState {
  final GameConfigState gameConfigState;

  AppState({@required this.gameConfigState});

  factory AppState.initial() {
    return AppState(gameConfigState: GameConfigState.initial());
  }
}
