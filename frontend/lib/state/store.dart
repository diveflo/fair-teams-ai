import 'package:frontend/state/appState.dart';
import 'package:redux/redux.dart';

final store = Store<AppState>(
  appReducer,
  initialState: AppState.initial(),
);
