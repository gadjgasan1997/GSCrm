class ProductCategory {
    Create(event) {
        return new Promise((resolve, reject) => {
            
        })
    }

    Update(event) {
        return new Promise((resolve, reject) => {
            
        })
    }

    Delete(event) {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("RemoveCategoryConfirmation")).then(dialogResult => {
                if (dialogResult.value) {
                    
                }
            })
        })
    }
}