/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow strict-local
 */

import React, {useState} from 'react';

import {
  SafeAreaView,
  ScrollView,
  StatusBar,
  Text,
  View,
  NativeModules,
} from 'react-native';
import {Button} from 'react-native-windows';
const getFile = () => {
  NativeModules.FileUp.FileUpld();
};

const App = () => {
  const [result, setResult] = useState();

  return (
    <SafeAreaView>
      <StatusBar />
      <ScrollView>
        <View>
          <Text>Hello</Text>
          <Button
            title="Get file"
            onPress={async () => {
              const pickerResult = await NativeModules.FileUp.FileUpld();
              setResult(pickerResult);
              console.log(JSON.stringify(result) );
            }}
          />
          <Text>{JSON.stringify(result)} </Text>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
};

export default App;
