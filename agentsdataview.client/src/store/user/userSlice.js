// src/store/user/userSlice.js
import { createSlice } from "@reduxjs/toolkit";

let currUser = JSON.parse(localStorage.getItem("currUser"));
try {
  if (currUser && Number(currUser.exp) <= Date.now()) {
    localStorage.removeItem("currUser");
    currUser = null;
  }
} catch {
  localStorage.removeItem("currUser");
  currUser = null;
}

const initialState = {
  currentUser: currUser,
};

const userSlice = createSlice({
  name: "user",
  initialState,
  reducers: {
    setCurrentUser(state, action) {
      state.currentUser = action.payload;
      localStorage.setItem("currUser", JSON.stringify(action.payload));
    },
    logout(state) {
      state.currentUser = null;
      localStorage.removeItem("currUser");
    },
  },
});

export const { setCurrentUser, logout } = userSlice.actions;
export default userSlice.reducer;
