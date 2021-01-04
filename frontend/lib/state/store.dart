import 'package:frontend/state/appState.dart';
import 'package:redux/redux.dart';
import 'package:redux_persist/redux_persist.dart';
import 'package:redux_persist_web/redux_persist_web.dart';
import 'package:redux_thunk/redux_thunk.dart';

Future<Store<AppState>> createStore() async {
  final persistor = Persistor<AppState>(
    storage: WebStorage(),
    serializer: JsonSerializer<AppState>(AppState.fromJson),
  );

  var initialState;
  try {
    initialState = await persistor.load();
  } catch (e) {
    initialState = null;
    print(e);
  }

  return Store<AppState>(appReducer,
      initialState: initialState ?? AppState.initial(),
      middleware: [persistor.createMiddleware(), thunkMiddleware]);
}
